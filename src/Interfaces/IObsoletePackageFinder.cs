namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IObsoletePackageFinder {
        void FindObsoletePackages(string solutionFolder, ErrorsAndInfos errorsAndInfos);
    }
}
