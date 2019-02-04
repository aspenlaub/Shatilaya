using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using NuGet.Packaging;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeInstaller : ICakeInstaller {
        public void InstallCake(IFolder cakeFolder, out IErrorsAndInfos errorsAndInfos) {
            var gitUtilities = new GitUtilities();
            errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/cake-build/example";
            var fixCakeErrorsAndInfos = new ErrorsAndInfos();
            gitUtilities.Clone(url, cakeFolder, new CloneOptions { BranchName = "master" }, true, () => File.Exists(CakeExeFileFullName(cakeFolder)), () => FixCakeVersionAndDownloadReadyToCake(cakeFolder, gitUtilities, fixCakeErrorsAndInfos), errorsAndInfos);
            errorsAndInfos.Errors.AddRange(fixCakeErrorsAndInfos.Errors);
        }

        private void FixCakeVersionAndDownloadReadyToCake(IFolder cakeFolder, IGitUtilities gitUtilities, IErrorsAndInfos errorsAndInfos) {
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

            gitUtilities.DownloadReadyToCake(cakeFolder.SubFolder(@"tools"), errorsAndInfos);
        }

        public string CakeExeFileFullName(IFolder cakeFolder) {
            return cakeFolder.SubFolder("tools").FullName + @"\Cake\cake.exe";
        }
    }
}
