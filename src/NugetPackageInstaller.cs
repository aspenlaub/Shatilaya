using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetPackageInstaller : INugetPackageInstaller {
        protected IComponentProvider ComponentProvider;

        public NugetPackageInstaller(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void InstallNugetPackage(IFolder packagesConfigFolder, string packageId, string version, bool excludeVersion, IErrorsAndInfos errorsAndInfos) {
            var arguments = new List<string> { "install", packageId };
            if (version != "") {
                arguments.Add("-Version \"" + version + "\"");
            }
            if (excludeVersion) {
                arguments.Add("-ExcludeVersion");
            }
            ComponentProvider.ProcessRunner.RunProcess("nuget.exe", string.Join(" ", arguments), packagesConfigFolder.FullName, errorsAndInfos);
        }
    }
}
