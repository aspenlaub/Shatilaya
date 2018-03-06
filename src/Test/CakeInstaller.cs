using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    public class CakeInstaller {
        public void InstallCake(IFolder cakeFolder) {
            const string url = "https://github.com/cake-build/example";
            Repository.Clone(url, cakeFolder.FullName, new CloneOptions { BranchName = "master" });
            var powershellExecuter = new PowershellExecuter();
            IList<string> errors;
            powershellExecuter.ExecutePowershellScriptFile(cakeFolder.FullName + @"\build.ps1", out errors);
            Assert.IsFalse(errors.Any());
            Assert.IsTrue(File.Exists(CakeExeFileFullName(cakeFolder)));
        }

        public string CakeExeFileFullName(IFolder cakeFolder) {
            return cakeFolder.FullName + @"\tools\Cake\cake.exe";
        }
    }
}
