using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("ReleaseBuild")]
[TaskDescription("Build solution in Release")]
public class ReleaseBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Release");
        context.MSBuild(context.SolutionFileFullName, settings
            => settings
               .SetConfiguration("Release")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
        );

    }
}