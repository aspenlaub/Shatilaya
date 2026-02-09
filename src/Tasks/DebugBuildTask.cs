using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("DebugBuild")]
[TaskDescription("Build solution in Debug")]
public class DebugBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Debug");
        context.MSBuild(context.SolutionFileFullName, settings
            => settings
               .SetConfiguration("Debug")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
        );

    }
}