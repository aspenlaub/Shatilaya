using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NuSpecCreator : INuSpecCreator {
        protected XNamespace NugetNamespace;
        protected XmlNamespaceManager NamespaceManager;
        protected IComponentProvider ComponentProvider;

        public NuSpecCreator(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
            NugetNamespace = XmlNamespaces.NuSpecNamespaceUri;
            NamespaceManager = new XmlNamespaceManager(new NameTable());
            NamespaceManager.AddNamespace("cp", XmlNamespaces.CsProjNamespaceUri);
        }

        public XDocument CreateNuSpec(string solutionFileFullName, IList<string> tags, IErrorsAndInfos errorsAndInfos) {
            var document = new XDocument();
            var projectFile = solutionFileFullName.Replace(".sln", ".csproj");
            if (!File.Exists(projectFile)) {
                errorsAndInfos.Errors.Add(string.Format(Texts.ProjectFileNotFound, projectFile));
                return document;
            }

            XDocument projectDocument;
            string namespaceSelector, targetFramework;
            try {
                projectDocument = XDocument.Load(projectFile);
                var targetFrameworkElement = projectDocument.XPathSelectElements("./Project/PropertyGroup/TargetFramework", NamespaceManager).FirstOrDefault();
                namespaceSelector = targetFrameworkElement != null ? "" : "cp:";
                targetFramework = targetFrameworkElement != null ? targetFrameworkElement.Value : "";
            } catch {
                errorsAndInfos.Errors.Add(string.Format(Texts.InvalidXmlFile, projectFile));
                return document;
            }

            var version = @"$version$";
            if (namespaceSelector == "") {
                var project = ComponentProvider.ProjectFactory.Load(solutionFileFullName, projectFile, errorsAndInfos);
                var releasePropertyGroup = project.PropertyGroups.FirstOrDefault(p => p.Condition.Contains("Release"));
                if (releasePropertyGroup != null) {
                    var solutionFolder = new Folder(solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\')));
                    var fullOutputFolder = new Folder(Path.Combine(solutionFolder.FullName, releasePropertyGroup.OutputPath));
                    var assemblyFileName = fullOutputFolder.FullName + '\\' + project.RootNamespace + ".dll";
                    if (File.Exists(assemblyFileName)) {
                        version = FileVersionInfo.GetVersionInfo(assemblyFileName).FileVersion;
                    }
                }
            }

            var dependencyIdsAndVersions = ComponentProvider.PackageConfigsScanner.DependencyIdsAndVersions(solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\') + 1), false, errorsAndInfos);
            var element = new XElement(NugetNamespace + "package");
            var solutionId = solutionFileFullName.Substring(solutionFileFullName.LastIndexOf('\\') + 1).Replace(".sln", "");
            var metaData = MetaData(solutionId, projectDocument, dependencyIdsAndVersions, tags, namespaceSelector, version, targetFramework, errorsAndInfos);
            if (metaData == null) {
                errorsAndInfos.Errors.Add(string.Format(Texts.MissingElementInProjectFile, projectFile));
                return document;
            }

            element.Add(metaData);
            var files = Files(projectDocument, namespaceSelector, errorsAndInfos);
            if (files == null) {
                errorsAndInfos.Errors.Add(string.Format(Texts.MissingElementInProjectFile, projectFile));
                return document;
            }

            element.Add(files);
            document.Add(element);
            return document;
        }

        protected XElement MetaData(string solutionId, XDocument projectDocument, IDictionary<string, string> dependencyIdsAndVersions, IList<string> tags, string namespaceSelector, string version, string targetFramework, IErrorsAndInfos errorsAndInfos) {
            var rootNamespaceElement = projectDocument.XPathSelectElements("./" + namespaceSelector + "Project/" + namespaceSelector + "PropertyGroup/" + namespaceSelector + "RootNamespace", NamespaceManager).FirstOrDefault();
            if (rootNamespaceElement == null) { return null; }

            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = ComponentProvider.PeghComponentProvider.SecretRepository.Get(developerSettingsSecret, errorsAndInfos);
            if (developerSettings == null) {
                errorsAndInfos.Errors.Add(string.Format(Texts.MissingDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return null;
            }

            var author = developerSettings.Author;
            var gitHubRepositoryUrl = developerSettings.GitHubRepositoryUrl;
            var faviconUrl = developerSettings.FaviconUrl;
            if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(gitHubRepositoryUrl) || string.IsNullOrEmpty(faviconUrl)) {
                errorsAndInfos.Errors.Add(string.Format(Texts.IncompleteDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return null;
            }

            var element = new XElement(NugetNamespace + @"metadata");
            foreach (var elementName in new[] { @"id", @"title", @"description", @"releaseNotes" }) {
                element.Add(new XElement(NugetNamespace + elementName, rootNamespaceElement.Value));
            }

            foreach (var elementName in new[] { @"authors", @"owners" }) {
                element.Add(new XElement(NugetNamespace + elementName, author));
            }

            element.Add(new XElement(NugetNamespace + @"projectUrl", gitHubRepositoryUrl + solutionId));
            element.Add(new XElement(NugetNamespace + @"iconUrl", faviconUrl));
            element.Add(new XElement(NugetNamespace + @"requireLicenseAcceptance", @"false"));
            var year = DateTime.Now.Year;
            element.Add(new XElement(NugetNamespace + @"copyright", $"Copyright {year}"));
            element.Add(new XElement(NugetNamespace + @"version", version));
            tags = tags.Where(t => !t.Contains('<') && !t.Contains('>') && !t.Contains('&') && !t.Contains(' ')).ToList();
            if (tags.Any()) {
                element.Add(new XElement(NugetNamespace + @"tags", string.Join(" ", tags)));
            }

            var dependenciesElement = new XElement(NugetNamespace + @"dependencies");
            element.Add(dependenciesElement);

            if (namespaceSelector == "") {
                var groupElement = new XElement(NugetNamespace + "group", new XAttribute("targetFramework", targetFramework));
                dependenciesElement.Add(groupElement);
                dependenciesElement = groupElement;
            }

            foreach (var dependencyElement in dependencyIdsAndVersions.Select(dependencyIdAndVersion
                => new XElement(NugetNamespace + @"dependency",
                    new XAttribute("id", dependencyIdAndVersion.Key), new XAttribute("version", dependencyIdAndVersion.Value)))) {
                dependenciesElement.Add(dependencyElement);
            }

            return element;
        }

        private static bool ParentIsReleasePropertyGroup(XElement e) {
            return e.Parent?.Attributes("Condition").Any(v => v.Value.Contains("Release")) == true;
        }

        protected XElement Files(XDocument projectDocument, string namespaceSelector, IErrorsAndInfos errorsAndInfos) {
            var rootNamespaceElement = projectDocument.XPathSelectElements("./" + namespaceSelector + "Project/" + namespaceSelector + "PropertyGroup/" + namespaceSelector + "RootNamespace", NamespaceManager).FirstOrDefault();
            if (rootNamespaceElement == null) { return null; }

            var outputPathElement = projectDocument.XPathSelectElements("./" + namespaceSelector + "Project/" + namespaceSelector + "PropertyGroup/" + namespaceSelector + "OutputPath", NamespaceManager).SingleOrDefault(ParentIsReleasePropertyGroup);
            if (outputPathElement == null) { return null; }

            var targetFrameworkElement = projectDocument.XPathSelectElements("./" + namespaceSelector + "Project/" + namespaceSelector + "PropertyGroup/" + namespaceSelector + "TargetFrameworkVersion", NamespaceManager).FirstOrDefault()
                ?? projectDocument.XPathSelectElements("./" + namespaceSelector + "Project/" + namespaceSelector + "PropertyGroup/" + namespaceSelector + "TargetFramework", NamespaceManager).FirstOrDefault();
            if (targetFrameworkElement == null) { return null; }

            var filesElement = new XElement(NugetNamespace + @"files");
            var topLevelNamespace = rootNamespaceElement.Value;
            if (!topLevelNamespace.Contains('.')) {
                errorsAndInfos.Errors.Add(string.Format(Texts.TopLevelNamespaceLacksADot, topLevelNamespace));
                return null;
            }

            topLevelNamespace = topLevelNamespace.Substring(0, topLevelNamespace.IndexOf('.'));
            foreach (var fileElement in new[] { @"dll", @"pdb" }.Select(extension
                  => new XElement(NugetNamespace + @"file",
                      new XAttribute(@"src", outputPathElement.Value + topLevelNamespace + ".*." + extension),
                      new XAttribute(@"exclude", string.Join(";", outputPathElement.Value + @"*.Test*.*", outputPathElement.Value + @"*.exe")),
                      new XAttribute(@"target", @"lib\net" + TargetFrameworkElementToLibNetSuffix(targetFrameworkElement))))) {
                filesElement.Add(fileElement);
            }

            return filesElement;
        }

        private static string TargetFrameworkElementToLibNetSuffix(XElement targetFrameworkElement) {
            var targetFramework = targetFrameworkElement.Value;
            return targetFramework.StartsWith("v") ? targetFramework.Replace("v", "").Replace(".", "") : targetFramework.StartsWith("net") ? targetFramework.Substring(3) : targetFramework;
        }

        public void CreateNuSpecFileIfRequiredOrPresent(bool required, string solutionFileFullName, IList<string> tags, IErrorsAndInfos errorsAndInfos) {
            var nuSpecFile = solutionFileFullName.Replace(".sln", ".nuspec");
            if (!required && !File.Exists(nuSpecFile)) { return; }

            var document = CreateNuSpec(solutionFileFullName, tags, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return; }

            var tempFileName = Path.GetTempPath() + @"\temp.nuspec";
            document.Save(tempFileName);
            if (File.Exists(nuSpecFile) && File.ReadAllText(nuSpecFile) == File.ReadAllText(tempFileName)) { return; }

            File.Copy(tempFileName, nuSpecFile, true);
            errorsAndInfos.Infos.Add(string.Format(Texts.NuSpecFileUpdated, nuSpecFile));
        }
    }
}
