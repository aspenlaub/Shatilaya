using System.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Core.Tooling;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;

public static class MsBuildSettingsExtensions {
    public static MSBuildSettings WithShatilayaCommonBuildSettings(this MSBuildSettings msBuildSettings, ShatilayaContext context) {
        return !File.Exists(context.MsBuildExeFullFileName)
            ? throw new FileNotFoundException(context.MsBuildExeFullFileName)
            : msBuildSettings
                .SetVerbosity(Verbosity.Minimal)
                .SetMSBuildPlatform(MSBuildPlatform.x64)
                .WithToolPath(context.MsBuildExeFullFileName);
    }
}
