using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    public class TestTargetFolder {
        protected string TestClassId, SolutionId;

        public TestTargetFolder(string testClassId, string solutionId) {
            TestClassId = testClassId;
            SolutionId = solutionId;
        }

        public IFolder Folder() {
            return new Folder(Path.GetTempPath() + TestClassId + @"\" + SolutionId);
        }

        public bool Exists() {
            return Folder().Exists();
        }

        public string FullName() {
            return Folder().FullName;
        }

        public IFolder MasterDebugBinFolder() {
            return Folder().ParentFolder().SubFolder(SolutionId + @"Bin/Debug");
        }

        public IFolder MasterReleaseBinFolder() {
            return Folder().ParentFolder().SubFolder(SolutionId + @"Bin/Release");
        }

        public void Delete() {
            var deleter = new FolderDeleter();

            if (Exists()) {
                deleter.DeleteFolder(Folder());
            }

            foreach (var folder in new[] { MasterDebugBinFolder(), MasterReleaseBinFolder() }.Where(folder => folder.Exists())) {
                deleter.DeleteFolder(folder);
            }
        }

        public void DeleteCakeFolder() {
            if (!CakeFolder().Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(CakeFolder());
        }

        public void RunBuildCakeScript(out IList<string> cakeMessages, out IList<string> cakeErrors) {
            var runner = new CakeRunner();
            var cakeExeFileFullName = CakeFolder().FullName + @"\tools\Cake\cake.exe";
            Assert.IsTrue(File.Exists(cakeExeFileFullName));
            var scriptFileFullName = FullName() + @"\build.cake";
            runner.CallCake(cakeExeFileFullName, scriptFileFullName, out cakeMessages, out cakeErrors);
        }

        public void CreateCakeFolder() {
            if (CakeFolder().Exists()) { return; }

            var cakeInstaller = new CakeInstaller();
            cakeInstaller.InstallCake(CakeFolder());
        }

        public IFolder CakeFolder() {
            return new Folder(Path.GetTempPath() + TestClassId + @"\Cake");
        }
    }
}
