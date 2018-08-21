using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using NuGet;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeInstaller : ICakeInstaller {
        public void InstallCake(IFolder cakeFolder, out IErrorsAndInfos errorsAndInfos) {
            var gitUtilities = new GitUtilities();
            errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/cake-build/example";
            var fixCakeErrorsAndInfos = new ErrorsAndInfos();
            gitUtilities.Clone(url, cakeFolder, new CloneOptions { BranchName = "master" }, true, () => File.Exists(CakeExeFileFullName(cakeFolder)), () => FixCakeVersionAndRunBuildPs1(cakeFolder, fixCakeErrorsAndInfos), errorsAndInfos);
            errorsAndInfos.Errors.AddRange(fixCakeErrorsAndInfos.Errors);
        }

        private void FixCakeVersionAndRunBuildPs1(IFolder cakeFolder, IErrorsAndInfos errorsAndInfos) {
            var packagesConfigFileFullName = cakeFolder.SubFolder("tools").FullName + @"\packages.config";
            var document = XDocument.Load(packagesConfigFileFullName);
            var element = document.XPathSelectElements("/packages/package").FirstOrDefault(e => e.Attribute("id")?.Value == "Cake");
            if (element == null) {
                errorsAndInfos.Errors.Add("Could not find package element");
                return;
            }
            var attribute = element.Attribute("version");
            if (attribute == null) {
                errorsAndInfos.Errors.Add("Could not find version attribute");
                return;
            }
            attribute.SetValue(CakeRunner.PinnedCakeVersion);
            document.Save(packagesConfigFileFullName);

            var powershellExecuter = new PowershellExecuter();
            IList<string> errors;
            powershellExecuter.ExecutePowershellScriptFile(cakeFolder.FullName + @"\build.ps1", out errors);
            errorsAndInfos.Errors.AddRange(errors);
            if (File.Exists(CakeExeFileFullName(cakeFolder))) {
                return;
            }

            errorsAndInfos.Errors.Add("Cake.exe not found");
        }

        public string CakeExeFileFullName(IFolder cakeFolder) {
            return cakeFolder.FullName + @"\tools\Cake\cake.exe";
        }
    }
}
