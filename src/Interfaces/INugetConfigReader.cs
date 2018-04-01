using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetConfigReader {
        string GetApiKey(string nugetConfigFileFullName, string source, IErrorsAndInfos errorsAndInfos);
    }
}
