using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetConfigReader {
        string GetApiKey(string nugetConfigFileFullName, string source, IErrorsAndInfos errorsAndInfos);
    }
}
