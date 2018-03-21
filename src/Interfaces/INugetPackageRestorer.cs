namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetPackageRestorer {
        void RestoreNugetPackages(string solutionFileFullName, ErrorsAndInfos errorsAndInfos);
    }
}
