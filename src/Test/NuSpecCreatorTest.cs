using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using PeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Components.ComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NuSpecCreatorTest {
        protected static TestTargetFolder PakledTarget = new TestTargetFolder(nameof(NuSpecCreator), "Pakled");
        protected static TestTargetFolder ChabStandardTarget = new TestTargetFolder(nameof(NuSpecCreator), "ChabStandard");

        protected XDocument Document;
        protected XmlNamespaceManager NamespaceManager;

        public NuSpecCreatorTest() {
            NamespaceManager = new XmlNamespaceManager(new NameTable());
            NamespaceManager.AddNamespace("cp", XmlNamespaces.CsProjNamespaceUri);
            NamespaceManager.AddNamespace("nu", XmlNamespaces.NuSpecNamespaceUri);
        }

        [TestInitialize]
        public void Initialize() {
            PakledTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestMethod]
        public void CanCreateNuSpecForPakled() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/Pakled.git";
            gitUtilities.Clone(url, PakledTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.PackageConfigsScanner).Returns(new PackageConfigsScanner());
            var peghComponentProvider = new PeghComponentProvider();
            componentProviderMock.SetupGet(c => c.PeghComponentProvider).Returns(peghComponentProvider);
            var sut = new NuSpecCreator(componentProviderMock.Object);
            var solutionFileFullName = PakledTarget.Folder().SubFolder("src").FullName + @"\" + PakledTarget.SolutionId + ".sln";
            var projectFileFullName = PakledTarget.Folder().SubFolder("src").FullName + @"\" + PakledTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            Document = XDocument.Load(projectFileFullName);
            var targetFrameworkElement = Document.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:TargetFrameworkVersion", NamespaceManager).FirstOrDefault();
            Assert.IsNotNull(targetFrameworkElement);
            var rootNamespaceElement = Document.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:RootNamespace", NamespaceManager).FirstOrDefault();
            Assert.IsNotNull(rootNamespaceElement);
            var outputPathElement = Document.XPathSelectElements("./cp:Project/cp:PropertyGroup/cp:OutputPath", NamespaceManager).SingleOrDefault(ParentIsReleasePropertyGroup);
            Assert.IsNotNull(outputPathElement);
            Document = sut.CreateNuSpec(solutionFileFullName, new List<string> { "Red", "White", "Blue", "Green<", "Orange&", "Violet>" }, errorsAndInfos);
            Assert.IsNotNull(Document);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(0, errorsAndInfos.Infos.Count);
            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = peghComponentProvider.SecretRepository.Get(developerSettingsSecret, errorsAndInfos);
            Assert.IsNotNull(developerSettings);
            VerifyTextElement(@"/package/metadata/id", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/title", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/description", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/releaseNotes", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/authors", developerSettings.Author);
            VerifyTextElement(@"/package/metadata/owners", developerSettings.Author);
            VerifyTextElement(@"/package/metadata/projectUrl", developerSettings.GitHubRepositoryUrl + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/iconUrl", developerSettings.FaviconUrl);
            VerifyTextElement(@"/package/metadata/requireLicenseAcceptance", @"false");
            var year = DateTime.Now.Year;
            VerifyTextElement(@"/package/metadata/copyright", $"Copyright {year}");
            VerifyTextElement(@"/package/metadata/version", @"$version$");
            VerifyElements(@"/package/metadata/dependencies/dependency", "id", new List<string> { "Newtonsoft.Json" });
            VerifyElements(@"/package/files/file", "src", new List<string> { @"..\..\PakledBin\Release\Aspenlaub.*.dll", @"..\..\PakledBin\Release\Aspenlaub.*.pdb" });
            VerifyElements(@"/package/files/file", "exclude", new List<string> { @"..\..\PakledBin\Release\*.Test*.*;..\..\PakledBin\Release\*.exe", @"..\..\PakledBin\Release\*.Test*.*;..\..\PakledBin\Release\*.exe" });
            var target = @"lib\net" + targetFrameworkElement.Value.Replace("v", "").Replace(".", "");
            VerifyElements(@"/package/files/file", "target", new List<string> { target, target });
            VerifyTextElement(@"/package/metadata/tags", @"Red White Blue");
        }

        [TestMethod]
        public void CanCreateNuSpecForChabStandard() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/ChabStandard.git";
            gitUtilities.Clone(url, ChabStandardTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.PackageConfigsScanner).Returns(new PackageConfigsScanner());
            var peghComponentProvider = new PeghComponentProvider();
            componentProviderMock.SetupGet(c => c.PeghComponentProvider).Returns(peghComponentProvider);
            var sut = new NuSpecCreator(componentProviderMock.Object);
            var solutionFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".sln";
            var projectFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            Document = XDocument.Load(projectFileFullName);
            var targetFrameworkElement = Document.XPathSelectElements("./Project/PropertyGroup/TargetFramework", NamespaceManager).FirstOrDefault();
            Assert.IsNotNull(targetFrameworkElement);
            var rootNamespaceElement = Document.XPathSelectElements("./Project/PropertyGroup/RootNamespace", NamespaceManager).FirstOrDefault();
            Assert.IsNotNull(rootNamespaceElement);
            var outputPathElement = Document.XPathSelectElements("./Project/PropertyGroup/OutputPath", NamespaceManager).SingleOrDefault(ParentIsReleasePropertyGroup);
            Assert.IsNotNull(outputPathElement);
            Document = sut.CreateNuSpec(solutionFileFullName, new List<string> { "Red", "White", "Blue", "Green<", "Orange&", "Violet>" }, errorsAndInfos);
            Assert.IsNotNull(Document);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(0, errorsAndInfos.Infos.Count);
            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = peghComponentProvider.SecretRepository.Get(developerSettingsSecret, errorsAndInfos);
            Assert.IsNotNull(developerSettings);
            VerifyTextElement(@"/package/metadata/id", @"Aspenlaub.Net.GitHub.CSharp." + ChabStandardTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/title", @"Aspenlaub.Net.GitHub.CSharp." + ChabStandardTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/description", @"Aspenlaub.Net.GitHub.CSharp." + ChabStandardTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/releaseNotes", @"Aspenlaub.Net.GitHub.CSharp." + ChabStandardTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/authors", developerSettings.Author);
            VerifyTextElement(@"/package/metadata/owners", developerSettings.Author);
            VerifyTextElement(@"/package/metadata/projectUrl", developerSettings.GitHubRepositoryUrl + ChabStandardTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/iconUrl", developerSettings.FaviconUrl);
            VerifyTextElement(@"/package/metadata/requireLicenseAcceptance", @"false");
            var year = DateTime.Now.Year;
            VerifyTextElement(@"/package/metadata/copyright", $"Copyright {year}");
            VerifyTextElement(@"/package/metadata/version", @"$version$");
            VerifyElements(@"/package/metadata/dependencies/dependency", "id", new List<string>());
            VerifyElements(@"/package/files/file", "src", new List<string> { @"..\..\ChabStandardBin\Release\Aspenlaub.*.dll", @"..\..\ChabStandardBin\Release\Aspenlaub.*.pdb" });
            VerifyElements(@"/package/files/file", "exclude", new List<string> { @"..\..\ChabStandardBin\Release\*.Test*.*;..\..\ChabStandardBin\Release\*.exe", @"..\..\ChabStandardBin\Release\*.Test*.*;..\..\ChabStandardBin\Release\*.exe" });
            var target = @"lib\" + targetFrameworkElement.Value;
            VerifyElements(@"/package/files/file", "target", new List<string> { target, target });
            VerifyTextElement(@"/package/metadata/tags", @"Red White Blue");
        }

        private static bool ParentIsReleasePropertyGroup(XElement e) {
            return e.Parent?.Attributes("Condition").Any(v => v.Value.Contains("Release")) == true;
        }

        protected void VerifyTextElement(string xpath, string expectedContents) {
            xpath = xpath.Replace("/", "/nu:");
            var element = Document.XPathSelectElements(xpath, NamespaceManager).FirstOrDefault();
            Assert.IsNotNull(element, $"Element not found using {xpath}, expected {expectedContents}");
        }

        protected void VerifyElements(string xpath, string attributeName, IList<string> attributeValues) {
            xpath = xpath.Replace("/", "/nu:");
            var elements = Document.XPathSelectElements(xpath, NamespaceManager).ToList();
            Assert.AreEqual(attributeValues.Count, elements.Count, $"Expected {attributeValues.Count} elements using {xpath}, got {elements.Count}");
            for (var i = 0; i < attributeValues.Count; i ++) {
                var element = elements[i];
                var attributeValue = attributeValues[i];
                var actualValue = element.Attribute(attributeName)?.Value;
                Assert.AreEqual(attributeValue, actualValue, $"Expected {attributeValue} for {attributeName} using {xpath}, got {actualValue}");
            }
        }
    }
}
