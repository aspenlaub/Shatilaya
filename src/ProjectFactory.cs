using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ProjectFactory : IProjectFactory {
        protected XmlNamespaceManager NamespaceManager;

        public ProjectFactory() {
            NamespaceManager = new XmlNamespaceManager(new NameTable());
            NamespaceManager.AddNamespace("cp", XmlNamespaces.CsProjNamespaceUri);
        }

        public IProject Load(string solutionFileFullName, string projectFileFullName, IErrorsAndInfos errorsAndInfos) {
            if (!File.Exists(solutionFileFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, solutionFileFullName));
                return null;
            }

            var projectFileInfo = new FileInfo(projectFileFullName);
            if (projectFileInfo.Directory == null) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, projectFileFullName));
                return null;
            }

            XDocument document;
            try {
                document = XDocument.Load(projectFileFullName);
            } catch {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.InvalidXmlFile, projectFileFullName));
                return null;
            }

            var projectElement = document.XPathSelectElements("./cp:Project", NamespaceManager).FirstOrDefault();

            var propertyGroups = document.XPathSelectElements("./cp:Project/cp:PropertyGroup", NamespaceManager).Select(x => ReadPropertyGroup(x)).ToList();
            var dllFileFullNames = document.XPathSelectElements("./cp:Project/cp:ItemGroup/cp:Reference/cp:HintPath", NamespaceManager).Select(x => DllFileFullName(projectFileInfo.DirectoryName, x));

            var project = new Project {
                ProjectFileFullName = projectFileFullName,
                ProjectName = ProjectName(solutionFileFullName, projectFileInfo),
                ToolsVersion = projectElement?.Attribute("ToolsVersion")?.Value ?? "",
                RootNamespace = propertyGroups.FirstOrDefault(p => p.RootNamespace != "")?.RootNamespace ?? ""
            };

            foreach (var propertyGroup in propertyGroups) {
                project.PropertyGroups.Add(propertyGroup);
            }

            foreach(var dllFileFullName in dllFileFullNames) {
                project.ReferencedDllFiles.Add(dllFileFullName);
            }

            return project;
        }

        protected string ProjectName(string solutionFileFullName, FileInfo projectFileInfo) {
            var solutionFolder = solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\'));
            foreach (var s in File.ReadAllLines(solutionFileFullName).ToList().Where(x => x.StartsWith("Project("))) {
                string projectName, projectFile;
                if (!ExtractProject(s, out projectName, out projectFile)) { continue; }

                var projectFullFileName = solutionFolder + '\\' + projectFile;
                if (!File.Exists(projectFullFileName)) { continue; }
                if (projectFullFileName != projectFileInfo.FullName) { continue; }

                return projectName;
            }

            return "";
        }

        protected bool ExtractProject(string s, out string projectName, out string projectFile) {
            projectName = projectFile = "";
            var pos = s.IndexOf("= \"", StringComparison.Ordinal);
            if (pos < 0) { return false; }

            s = s.Remove(0, 3 + pos);
            projectName = s.Substring(0, s.IndexOf("\"", StringComparison.Ordinal));
            pos = s.IndexOf(", \"", StringComparison.Ordinal);
            if (pos < 0) { return false; }

            s = s.Remove(0, 3 + s.IndexOf(", \"", StringComparison.Ordinal));
            projectFile = s.Substring(0, s.IndexOf("\"", StringComparison.Ordinal));
            return true;
        }

        protected IPropertyGroup ReadPropertyGroup(XElement propertyGroupElement) {
            var propertyGroup = new PropertyGroup {
                AssemblyName = propertyGroupElement?.XPathSelectElement("cp:AssemblyName", NamespaceManager)?.Value ?? "",
                RootNamespace = propertyGroupElement?.XPathSelectElement("cp:RootNamespace", NamespaceManager)?.Value ?? "",
                OutputPath = propertyGroupElement?.XPathSelectElement("cp:OutputPath", NamespaceManager)?.Value ?? "",
                IntermediateOutputPath = propertyGroupElement?.XPathSelectElement("cp:IntermediateOutputPath", NamespaceManager)?.Value ?? "",
                UseVsHostingProcess = propertyGroupElement?.XPathSelectElement("cp:UseVSHostingProcess", NamespaceManager)?.Value ?? "",
                GenerateBuildInfoConfigFile = propertyGroupElement?.XPathSelectElement("cp:GenerateBuildInfoConfigFile", NamespaceManager)?.Value ?? "",
                Condition = propertyGroupElement?.Attribute("Condition")?.Value ?? ""
            };
            return propertyGroup;
        }

        protected string DllFileFullName(string projectFolderFullName, XElement hintPathElement) {
            return projectFolderFullName + '\\' + hintPathElement.Value;
        }
    }
}
