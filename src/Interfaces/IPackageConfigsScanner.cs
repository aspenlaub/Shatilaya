using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IPackageConfigsScanner {
        IDictionary<string, string> DependencyIdsAndVersions(string projectFolder, bool includeTest, IErrorsAndInfos errorsAndInfos);
    }
}