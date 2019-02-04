using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetPackageInstallerTest {
        protected static TestTargetFolder ChabStandardTarget = new TestTargetFolder(nameof(NugetPackageInstallerTest), "ChabStandard");
        protected IComponentProvider ComponentProvider;

        public NugetPackageInstallerTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ChabStandardTarget.DeleteCakeFolder();
            ChabStandardTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            ChabStandardTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            ChabStandardTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            ChabStandardTarget.Delete();
        }

        [TestMethod]
        public void CanInstallNugetPackage() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            var url = "https://github.com/aspenlaub/" + ChabStandardTarget.SolutionId + ".git";
            gitUtilities.Clone(url, ChabStandardTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsFalse(ChabStandardTarget.Folder().SubFolder(@"src\OctoPack.3.6.0").Exists());
            var sut = new NugetPackageInstaller(ComponentProvider);

            sut.InstallNugetPackage(ChabStandardTarget.Folder().SubFolder("src"), "OctoPack", "3.6.0", false, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains("Adding package") && i.Contains("to folder")));
            Assert.IsTrue(ChabStandardTarget.Folder().SubFolder(@"src\OctoPack.3.6.0").Exists());

            errorsAndInfos = new ErrorsAndInfos();
            const string oldCakeVersion = "0.24.0";
            sut.InstallNugetPackage(ChabStandardTarget.Folder().SubFolder("tools"), "Cake", oldCakeVersion, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsTrue(ChabStandardTarget.Folder().SubFolder(@"tools\Cake").Exists());
            Assert.IsTrue(File.Exists(ChabStandardTarget.Folder().SubFolder(@"tools\Cake").FullName + @"\Cake.exe"));

            errorsAndInfos = new ErrorsAndInfos();
            sut.InstallNugetPackage(ChabStandardTarget.Folder().SubFolder("tools"), "Cake", "", true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains("Successfully uninstalled") && i.Contains(oldCakeVersion)));
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains("Successfully installed")));
        }
    }
}
