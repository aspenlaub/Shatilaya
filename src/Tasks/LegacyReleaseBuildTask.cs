using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("LegacyReleaseBuild")]
[TaskDescription("Build legacy solution in Release")]
public class LegacyReleaseBuildTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building legacy solution in Release");
        context.MSBuild(context.LegacySolutionFileFullName, settings
            => settings
               .SetConfiguration("Release")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
        );

    }
}