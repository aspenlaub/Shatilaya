﻿using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ObsoletePackageFinderTest {
        protected static TestTargetFolder ChabTarget = new TestTargetFolder(nameof(ObsoletePackageFinderTest), "Chab");
        protected IComponentProvider ComponentProvider;

        public ObsoletePackageFinderTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ChabTarget.DeleteCakeFolder();
            ChabTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            ChabTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            ChabTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            ChabTarget.Delete();
        }

        [TestMethod]
        public void CanFindObsoletePackages() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            var url = "https://github.com/aspenlaub/" + ChabTarget.SolutionId + ".git";
            gitUtilities.Clone(url, ChabTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            var latestBuildScriptProvider = new LatestBuildCakeScriptProvider();
            var cakeScript = latestBuildScriptProvider.GetLatestBuildCakeScript();
            cakeScript = CakeBuildUtilities.UseLocalShatilayaAssemblies(cakeScript);
            var cakeScriptFileFullName = ChabTarget.Folder().FullName + @"\build.cake";
            File.WriteAllText(cakeScriptFileFullName, cakeScript);

            ChabTarget.RunBuildCakeScript(ComponentProvider, "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            errorsAndInfos = new ErrorsAndInfos();
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.Setup(c => c.PackageConfigsScanner).Returns(new PackageConfigsScanner());
            IObsoletePackageFinder sut = new ObsoletePackageFinder(componentProviderMock.Object);
            var solutionFolder = ChabTarget.Folder().SubFolder("src");
            sut.FindObsoletePackages(solutionFolder.FullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsFalse(errorsAndInfos.Infos.Any());
            var obsoleteFolder = solutionFolder.SubFolder(@"packages\ObsoPack");
            Directory.CreateDirectory(obsoleteFolder.FullName);
            foreach(var extension in new[] { "cs", "dll", "json", "_", "cake", "php", "txt", "docx", "exe", "js", "css", "bat", "cmd", "xlsx", "csv", "sln" }) {
                File.WriteAllText(obsoleteFolder.FullName + "\\somefile." + extension, "Delete me");
            }
            File.WriteAllText(obsoleteFolder.FullName + "\\somefile.csproj", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Project ToolsVersion=\"14.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"></Project>");

            sut.FindObsoletePackages(solutionFolder.FullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains(obsoleteFolder.FullName) && i.Contains("has been deleted")));
        }
    }
}
