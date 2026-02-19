using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("ReleaseBuild")]
[TaskDescription("Build solution in Release")]
[IsDependentOn(typeof(InitializeContextForMsBuildTask))]
public class ReleaseBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Release");
        context.MSBuild(context.SolutionFileFullName, settings
            => settings
               .WithShatilayaCommonBuildSettings(context)
               .SetConfiguration("Release")
        );
    }
}