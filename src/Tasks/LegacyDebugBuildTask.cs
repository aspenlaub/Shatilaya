using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("LegacyDebugBuild")]
[TaskDescription("Build legacy solution in Debug")]
[IsDependentOn(typeof(InitializeContextTask))]
public class LegacyDebugBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building legacy solution in Debug");
        context.MSBuild(context.LegacySolutionFileFullName, settings
            => settings
               .WithShatilayaCommonBuildSettings(context)
               .SetConfiguration("Debug")
        );

    }
}