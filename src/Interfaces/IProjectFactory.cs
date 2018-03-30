namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IProjectFactory {
        IProject Load(string solutionFileFullName, string projectFileFullName, ErrorsAndInfos errorsAndInfos);
    }
}
