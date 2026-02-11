using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("LegacyDebugBuild")]
[TaskDescription("Build legacy solution in Debug")]
public class LegacyDebugBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building legacy solution in Debug");
        context.MSBuild(context.LegacySolutionFileFullName, settings
            => settings
               .SetConfiguration("Debug")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
        );

    }
}