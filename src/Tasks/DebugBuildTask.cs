using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("DebugBuild")]
[TaskDescription("Build solution in Debug")]
[IsDependentOn(typeof(InitializeContextTask))]
public class DebugBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Debug");
        context.MSBuild(context.SolutionFileFullName, settings
            => settings
               .WithShatilayaCommonBuildSettings(context)
               .SetConfiguration("Debug")
        );

    }
}