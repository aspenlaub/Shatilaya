using System.IO;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetPackageRestorer : INugetPackageRestorer {
        protected IComponentProvider ComponentProvider;

        public NugetPackageRestorer(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void RestoreNugetPackages(string solutionFileFullName, IErrorsAndInfos errorsAndInfos) {
            var directoryName = solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\'));
            if (!Directory.Exists(directoryName + @"\packages")) { return; }

            ComponentProvider.ProcessRunner.RunProcess("nuget.exe", "restore " + solutionFileFullName, directoryName, errorsAndInfos);
        }
    }
}
