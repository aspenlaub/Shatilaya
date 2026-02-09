using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("BuildAndTestDebugAndRelease")]
[TaskDescription("Build and test debug and release configuration")]
[IsDependentOn(typeof(InitializeContextTask))]
[IsDependentOn(typeof(DebugBuildTask))]
[IsDependentOn(typeof(RunTestsOnDebugArtifactsTask))]
[IsDependentOn(typeof(CopyDebugArtifactsTask))]
[IsDependentOn(typeof(ReleaseBuildTask))]
[IsDependentOn(typeof(RunTestsOnReleaseArtifactsTask))]
[IsDependentOn(typeof(CopyReleaseArtifactsTask))]
public class BuildAndTestDebugAndReleaseTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building and testing debug and release configuration");
    }
}