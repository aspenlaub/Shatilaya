using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("DebugBuildToTemp")]
[TaskDescription("Build solution in Debug into a temporary folder")]
public class DebugBuildToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Debug into a temporary folder");
        IFolder tempFolder = context.SolutionFolderWithinOrOutsideSrc.SubFolder("temp").SubFolder("bin").SubFolder("Debug");
        context.Information($"Output folder is: {tempFolder.FullName}");
        context.MSBuild(context.SolutionFileFullName, settings
            => settings
               .SetConfiguration("Debug")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
               .WithProperty("OutDir", tempFolder.FullName)
        );

    }
}