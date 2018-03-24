using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class CakeRunnerTest {
        protected ICakeRunner Sut;
        protected static string CakeExeFileFullName;
        protected static IFolder ShatilFolderFullName;

        protected const string ThisIsNotCake = @"This is not a cake!";

        [TestInitialize]
        public void Initialize() {
            DeleteFolder(CakeFolder());
            DeleteFolder(ShatilFolder());
            var cakeInstaller = new CakeInstaller();
            cakeInstaller.InstallCake(CakeFolder());
            CakeExeFileFullName = cakeInstaller.CakeExeFileFullName(CakeFolder());

            ShatilFolderFullName = ShatilFolder();
            GitTestUtilities.MakeSureGit2AssembliesAreInPlace();
            const string url = "https://github.com/aspenlaub/Shatilaya.git";
            Repository.Clone(url, ShatilFolderFullName.FullName, new CloneOptions { BranchName = "master" });

            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            Sut = new CakeRunner(componentProviderMock.Object);
        }

        [TestCleanup]
        public void Cleanup() {
            DeleteFolder(CakeFolder());
            DeleteFolder(ShatilFolder());
        }

        private static void DeleteFolder(IFolder folder) {
            if (!folder.Exists()) {
                return;
            }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }

        protected static IFolder CakeFolder() {
            return new Folder(Path.GetTempPath() + @"Cake");
        }

        protected static IFolder ShatilFolder() {
            return new Folder(Path.GetTempPath() + @"Shatil");
        }

        [TestMethod]
        public void CanCallScriptWithoutErrors() {
            var errorsAndInfos = new ErrorsAndInfos();

            Sut.CallCake(CakeExeFileFullName, ShatilFolderFullName.FullName + @"\src\Test\success.cake", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Task")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Duration")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"00:00:00")));
        }

        [TestMethod]
        public void CanCallScriptWithErrors() {
            var errorsAndInfos = new ErrorsAndInfos();

            Sut.CallCake(CakeExeFileFullName, ShatilFolderFullName.FullName + @"\src\Test\failure.cake", errorsAndInfos);
            Assert.AreEqual(1, errorsAndInfos.Errors.Count);
            Assert.AreEqual("This is not a cake!", errorsAndInfos.Errors[0]);
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Task")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Duration")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"00:00:00")));
            Assert.IsFalse(errorsAndInfos.Infos.Any(m => m.Contains(ThisIsNotCake)));
        }
    }
}
