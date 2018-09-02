using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IProjectFactory {
        IProject Load(string solutionFileFullName, string projectFileFullName, IErrorsAndInfos errorsAndInfos);
    }
}
