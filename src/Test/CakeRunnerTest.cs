using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class CakeRunnerTest {
        protected ICakeRunner Sut;
        protected static string CakeExeFileFullName;
        protected static IFolder ShatilFolderFullName;

        [TestInitialize]
        public void Initialize() {
            DeleteFolder(CakeFolder());
            DeleteFolder(ShatilFolder());
            var cakeFolder = CakeFolder();
            var url = "https://github.com/cake-build/example";
            Repository.Clone(url, cakeFolder.FullName, new CloneOptions { BranchName = "master" });
            var powershellExecuter = new PowershellExecuter();
            IList<string> errors;
            powershellExecuter.ExecutePowershellScriptFile(cakeFolder.FullName + @"\build.ps1", out errors);
            Assert.IsFalse(errors.Any());
            CakeExeFileFullName = cakeFolder.FullName + @"\tools\Cake\cake.exe";
            Assert.IsTrue(File.Exists(CakeExeFileFullName));

            ShatilFolderFullName = ShatilFolder();
            url = "https://github.com/aspenlaub/Shatilaya.git";
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
            IList<string> errors;

            Sut.CallCake(CakeExeFileFullName, ShatilFolderFullName.FullName + @"\src\Test\success.cake", out errors);
            Assert.IsFalse(errors.Any());
        }

        [TestMethod]
        public void CanCallScriptWithErrors() {
            IList<string> errors;

            Sut.CallCake(CakeExeFileFullName, ShatilFolderFullName.FullName + @"\src\Test\failure.cake", out errors);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("This is not a cake!", errors[0]);
        }
    }
}
