using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetPackageRestorer : INugetPackageRestorer {
        protected IComponentProvider ComponentProvider;

        public NugetPackageRestorer(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void RestoreNugetPackages(string solutionFileFullName, ErrorsAndInfos errorsAndInfos) {
            var directoryName = solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\'));
            if (!Directory.Exists(directoryName + @"\packages")) { return; }

            ComponentProvider.ProcessRunner.RunProcess("nuget.exe", "restore " + solutionFileFullName, directoryName, errorsAndInfos);
        }
    }
}
