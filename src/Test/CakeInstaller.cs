using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
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
            gitUtilities.Clone(url, cakeFolder, new CloneOptions { BranchName = "master" }, true, () => File.Exists(CakeExeFileFullName(cakeFolder)), () => FixCakeVersionAndRunBuildPs1(cakeFolder), errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        private void FixCakeVersionAndRunBuildPs1(IFolder cakeFolder) {
            var packagesConfigFileFullName = cakeFolder.SubFolder("tools").FullName + @"\packages.config";
            var document = XDocument.Load(packagesConfigFileFullName);
            var element = document.XPathSelectElements("/packages/package").FirstOrDefault(e => e.Attribute("id")?.Value == "Cake");
            Assert.IsNotNull(element);
            var attribute = element.Attribute("version");
            Assert.IsNotNull(attribute);
            attribute.SetValue(CakeRunner.PinnedCakeVersion);
            document.Save(packagesConfigFileFullName);

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
