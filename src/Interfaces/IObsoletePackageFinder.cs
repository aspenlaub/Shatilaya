using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IObsoletePackageFinder {
        void FindObsoletePackages(string solutionFolder, IErrorsAndInfos errorsAndInfos);
    }
}
