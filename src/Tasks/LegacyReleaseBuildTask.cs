using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("LegacyReleaseBuild")]
[TaskDescription("Build legacy solution in Release")]
[IsDependentOn(typeof(InitializeContextForMsBuildTask))]
public class LegacyReleaseBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building legacy solution in Release");
        context.MSBuild(context.LegacySolutionFileFullName, settings
            => settings
               .WithShatilayaCommonBuildSettings(context)
               .SetConfiguration("Release")
        );

    }
}