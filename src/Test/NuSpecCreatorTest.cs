using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NuSpecCreatorTest {
        protected static TestTargetFolder PakledTarget = new TestTargetFolder(nameof(NuSpecCreator), "Pakled");

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
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledTarget.Delete();
        }

        [TestMethod]
        public void CanCreateNuSpecForPakled() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/Pakled.git";
            gitUtilities.Clone(url, PakledTarget.Folder(), new CloneOptions { BranchName = "master" }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.PackageConfigsScanner).Returns(new PackageConfigsScanner());
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
            Document = sut.CreateNuSpec(solutionFileFullName, errorsAndInfos);
            Assert.IsNotNull(Document);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(0, errorsAndInfos.Infos.Count);
            VerifyTextElement(@"/package/metadata/id", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/title", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/description", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/releaseNotes", @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/authors", @"Wolfgang Berger (aspenlaub.net)");
            VerifyTextElement(@"/package/metadata/owners", @"Wolfgang Berger (aspenlaub.net)");
            VerifyTextElement(@"/package/metadata/projectUrl", @"https://github.com/aspenlaub/" + PakledTarget.SolutionId);
            VerifyTextElement(@"/package/metadata/iconUrl", @"https://www.aspenlaub.net/favicon.ico");
            VerifyTextElement(@"/package/metadata/requireLicenseAcceptance", @"false");
            var year = DateTime.Now.Year;
            VerifyTextElement(@"/package/metadata/copyright", $"Copyright {year}");
            VerifyTextElement(@"/package/metadata/version", @"$version$");
            VerifyElements(@"/package/metadata/dependencies/dependency", "id", new List<string> { "Newtonsoft.Json" });
            VerifyElements(@"/package/files/file", "src", new List<string> { @"..\PakledBin\Release\Aspenlaub.*.dll", @"..\PakledBin\Release\Aspenlaub.*.pdb" });
            VerifyElements(@"/package/files/file", "exclude", new List<string> { @"..\PakledBin\Release\*.Test*.*", @"..\PakledBin\Release\*.Test*.*" });
            var target = @"lib\net" + targetFrameworkElement.Value.Replace("v", "").Replace(".", "");
            VerifyElements(@"/package/files/file", "target", new List<string> { target, target });
        }

        private static bool ParentIsReleasePropertyGroup(XElement e) {
            return e.Parent?.Attributes("Condition").Any(v => v.Value.Contains("Release")) == true;
        }

        protected void VerifyTextElement(string xpath, string expectedContents) {
            xpath = xpath.Replace("/", "/nu:");
            var element = Document.XPathSelectElements(xpath, NamespaceManager).FirstOrDefault();
            Assert.IsNotNull(element);
        }

        protected void VerifyElements(string xpath, string attributeName, IList<string> attributeValues) {
            xpath = xpath.Replace("/", "/nu:");
            var elements = Document.XPathSelectElements(xpath, NamespaceManager).ToList();
            Assert.AreEqual(attributeValues.Count, elements.Count);
        }
    }
}
