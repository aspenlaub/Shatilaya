using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IObsoletePackageFinder {
        void FindObsoletePackages(string solutionFolder, IErrorsAndInfos errorsAndInfos);
    }
}
