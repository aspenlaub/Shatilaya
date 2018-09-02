using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetPackageRestorer {
        void RestoreNugetPackages(string solutionFileFullName, IErrorsAndInfos errorsAndInfos);
    }
}
