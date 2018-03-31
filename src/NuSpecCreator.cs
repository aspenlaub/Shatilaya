using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NuSpecCreator : INuSpecCreator {
        protected XNamespace Namespace;
        protected XmlNamespaceManager NamespaceManager;
        protected IComponentProvider ComponentProvider;

        public NuSpecCreator(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
            Namespace = XmlNamespaces.NuSpecNamespaceUri;
            NamespaceManager = new XmlNamespaceManager(new NameTable());
            NamespaceManager.AddNamespace("cp", XmlNamespaces.CsProjNamespaceUri);
        }

        public XDocument CreateNuSpec(string solutionFileFullName, ErrorsAndInfos errorsAndInfos) {
            var document = new XDocument();
            var projectFile = solutionFileFullName.Replace(".sln", ".csproj");
            if (!File.Exists(projectFile)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.ProjectFileNotFound, projectFile));
                return document;
            }

            XDocument projectDocument;
            try {
                projectDocument = XDocument.Load(projectFile);
            } catch {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.InvalidXmlFile, projectFile));
                return document;
            }

            var dependencyIdsAndVersions = ComponentProvider.PackageConfigsScanner.DependencyIdsAndVersions(solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\') + 1), false, errorsAndInfos);
            var element = new XElement(Namespace + "package");
            var solutionId = solutionFileFullName.Substring(solutionFileFullName.LastIndexOf('\\') + 1).Replace(".sln", "");
            var metaData = MetaData(solutionId, projectDocument, dependencyIdsAndVersions, errorsAndInfos);
            if (metaData == null) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.MissingElementInProjectFile, projectFile));
                return document;
            }

            element.Add(metaData);
            var files = Files(projectDocument, errorsAndInfos);
            if (files == null) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.MissingElementInProjectFile, projectFile));
                return document;
            }

            element.Add(files);
            document.Add(element);
            return document;
        }

        protected XElement MetaData(string solutionId, XDocument projectDocument, IDictionary<string, string> dependencyIdsAndVersions, ErrorsAndInfos errorsAndInfos) {
            var rootNamespaceElement = projectDocument.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:RootNamespace", NamespaceManager).FirstOrDefault();
            if (rootNamespaceElement == null) { return null; }

            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = ComponentProvider.PeghComponentProvider.SecretRepository.Get(developerSettingsSecret);
            if (developerSettings == null) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.MissingDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return null;
            }

            var element = new XElement(Namespace + @"metadata");
            foreach (var elementName in new[] { @"id", @"title", @"description", @"releaseNotes" }) {
                element.Add(new XElement(Namespace + elementName, rootNamespaceElement.Value));
            }

            foreach (var elementName in new[] { @"authors", @"owners" }) {
                element.Add(new XElement(Namespace + elementName, developerSettings.Author));
            }

            element.Add(new XElement(Namespace + @"projectUrl", developerSettings.GitHubRepositoryUrl + solutionId));
            element.Add(new XElement(Namespace + @"iconUrl", developerSettings.FaviconUrl));
            element.Add(new XElement(Namespace + @"requireLicenseAcceptance", @"false"));
            var year = DateTime.Now.Year;
            element.Add(new XElement(Namespace + @"copyright", $"Copyright {year}"));
            element.Add(new XElement(Namespace + @"version", @"$version$"));

            var dependenciesElement = new XElement(Namespace + @"dependencies");
            foreach (var dependencyElement in dependencyIdsAndVersions.Select(dependencyIdAndVersion
                => new XElement(Namespace + @"dependency",
                    new XAttribute("id", dependencyIdAndVersion.Key), new XAttribute("version", dependencyIdAndVersion.Value)))) {
                dependenciesElement.Add(dependencyElement);
            }

            element.Add(dependenciesElement);
            return element;
        }

        private static bool ParentIsReleasePropertyGroup(XElement e) {
            return e.Parent?.Attributes("Condition").Any(v => v.Value.Contains("Release")) == true;
        }

        protected XElement Files(XDocument projectDocument, ErrorsAndInfos errorsAndInfos) {
            var rootNamespaceElement = projectDocument.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:RootNamespace", NamespaceManager).FirstOrDefault();
            if (rootNamespaceElement == null) { return null; }

            var outputPathElement = projectDocument.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:OutputPath", NamespaceManager).SingleOrDefault(ParentIsReleasePropertyGroup);
            if (outputPathElement == null) { return null; }

            var targetFrameworkElement = projectDocument.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:TargetFrameworkVersion", NamespaceManager).FirstOrDefault();
            if (targetFrameworkElement == null) { return null; }

            var filesElement = new XElement(Namespace + @"files");
            var topLevelNamespace = rootNamespaceElement.Value;
            if (!topLevelNamespace.Contains('.')) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.TopLevelNamespaceLacksADot, topLevelNamespace));
                return null;
            }

            topLevelNamespace = topLevelNamespace.Substring(0, topLevelNamespace.IndexOf('.'));
            foreach (var fileElement in new[] { @"dll", @"pdb" }.Select(extension
                  => new XElement(Namespace + @"file",
                      new XAttribute(@"src", outputPathElement.Value + topLevelNamespace + ".*." + extension),
                      new XAttribute(@"exclude", outputPathElement.Value + @"*.Test*.*"),
                      new XAttribute(@"target", @"lib\net" + targetFrameworkElement.Value.Replace("v", "").Replace(".", ""))))) {
                filesElement.Add(fileElement);
            }

            return filesElement;
        }

        public void CreateNuSpecFileIfRequiredOrPresent(bool required, string solutionFileFullName, ErrorsAndInfos errorsAndInfos) {
            var nuSpecFile = solutionFileFullName.Replace(".sln", ".nuspec");
            if (!required && !File.Exists(nuSpecFile)) { return; }

            var document = CreateNuSpec(solutionFileFullName, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return; }

            const string tempFileName = @"c:\temp\temp.nuspec";
            document.Save(tempFileName);
            if (File.Exists(nuSpecFile) && File.ReadAllText(nuSpecFile) == File.ReadAllText(tempFileName)) { return; }

            File.Copy(tempFileName, nuSpecFile, true);
            errorsAndInfos.Infos.Add(string.Format(Properties.Resources.NuSpecFileUpdated, nuSpecFile));
        }
    }
}
