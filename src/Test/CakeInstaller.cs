using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    public class CakeInstaller {
        public void InstallCake(IFolder cakeFolder) {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/cake-build/example";
            gitUtilities.Clone(url, cakeFolder, new CloneOptions { BranchName = "master" }, true, () => RunBuildPs1(cakeFolder), errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        private void RunBuildPs1(IFolder cakeFolder) {
            var powershellExecuter = new PowershellExecuter();
            IList<string> errors;
            powershellExecuter.ExecutePowershellScriptFile(cakeFolder.FullName + @"\build.ps1", out errors);
            Assert.IsFalse(errors.Any(), string.Join("\r\n", errors));
            Assert.IsTrue(File.Exists(CakeExeFileFullName(cakeFolder)));
        }

        public string CakeExeFileFullName(IFolder cakeFolder) {
            return cakeFolder.FullName + @"\tools\Cake\cake.exe";
        }
    }
}
