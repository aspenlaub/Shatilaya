using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IProjectFactory {
        IProject Load(string solutionFileFullName, string projectFileFullName, IErrorsAndInfos errorsAndInfos);
    }
}
