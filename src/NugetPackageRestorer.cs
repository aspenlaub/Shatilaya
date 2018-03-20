using System.Diagnostics;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetPackageRestorer : INugetPackageRestorer {
        private const int WaitForHowManySeconds = 10;

        public void RestoreNugetPackages(string solutionFileFullName) {
            var directoryName = solutionFileFullName.Substring(0, solutionFileFullName.LastIndexOf('\\'));
            if (!Directory.Exists(directoryName + @"\packages")) { return; }

            var process = new Process();
            var startInfo = new ProcessStartInfo {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "nuget.exe",
                Arguments = "restore " + solutionFileFullName,
                WorkingDirectory = directoryName,
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(WaitForHowManySeconds * 1000);
        }
    }
}
