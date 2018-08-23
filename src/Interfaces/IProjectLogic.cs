namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IProjectLogic {
        bool IsANetStandardOrCoreProject(IProject project);
        bool DoAllNetStandardOrCoreConfigurationsHaveNuspecs(IProject project);
    }
}
