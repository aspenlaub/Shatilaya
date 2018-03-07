using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

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
            const string url = "https://github.com/aspenlaub/Shatilaya.git";
            Repository.Clone(url, ShatilFolderFullName.FullName, new CloneOptions { BranchName = "master" });

            Sut = new CakeRunner();
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
            IList<string> messages, errors;

            Sut.CallCake(CakeExeFileFullName, ShatilFolderFullName.FullName + @"\src\Test\success.cake", out messages, out errors);
            Assert.IsFalse(errors.Any());
            Assert.IsTrue(messages.Any(m => m.Contains(@"Task")));
            Assert.IsTrue(messages.Any(m => m.Contains(@"Duration")));
            Assert.IsTrue(messages.Any(m => m.Contains(@"00:00:00")));
        }

        [TestMethod]
        public void CanCallScriptWithErrors() {
            IList<string> messages, errors;

            Sut.CallCake(CakeExeFileFullName, ShatilFolderFullName.FullName + @"\src\Test\failure.cake", out messages, out errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("This is not a cake!", errors[0]);
            Assert.IsTrue(messages.Any(m => m.Contains(@"Task")));
            Assert.IsTrue(messages.Any(m => m.Contains(@"Duration")));
            Assert.IsTrue(messages.Any(m => m.Contains(@"00:00:00")));
            Assert.IsFalse(messages.Any(m => m.Contains(ThisIsNotCake)));
        }
    }
}
