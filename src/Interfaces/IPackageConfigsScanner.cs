using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IPackageConfigsScanner {
        IDictionary<string, string> DependencyIdsAndVersions(string projectFolder, bool includeTest, ErrorsAndInfos errorsAndInfos);
    }
}