using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ObsoletePackageFinderTest {
        protected static TestTargetFolder ChabTarget = new TestTargetFolder(nameof(ObsoletePackageFinderTest), "Chab");

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
            var url = "https://github.com/aspenlaub/" + ChabTarget.SolutionId + ".git";
            Repository.Clone(url, ChabTarget.FullName(), new CloneOptions { BranchName = "master" });
            IList<string> cakeMessages, cakeErrors;
            ChabTarget.RunBuildCakeScript(out cakeMessages, out cakeErrors);
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.Setup(c => c.PackageConfigsScanner).Returns(new PackageConfigsScanner());
            var sut = new ObsoletePackageFinder(componentProviderMock.Object);
            var errorsAndInfos = new ErrorsAndInfos();
            var solutionFolder = ChabTarget.Folder().SubFolder("src");
            sut.FindObsoletePackages(solutionFolder.FullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsFalse(errorsAndInfos.Infos.Any());
            var obsoleteFolder = solutionFolder.SubFolder(@"packages\ObsoPack");
            Directory.CreateDirectory(obsoleteFolder.FullName);
            sut.FindObsoletePackages(solutionFolder.FullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains(obsoleteFolder.FullName) && i.Contains("has been deleted")));
        }
    }
}
