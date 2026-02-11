using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("ReleaseBuildToTemp")]
[TaskDescription("Build solution in Release into a temporary folder")]
public class ReleaseBuildToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Release into a temporary folder");
        IFolder tempFolder = context.SolutionFolderWithinOrOutsideSrc.SubFolder("temp").SubFolder("bin").SubFolder("Release");
        context.Information($"Output folder is: {tempFolder.FullName}");
        tempFolder.CreateIfNecessary();
        context.MSBuild(context.SolutionFileFullName, settings
            => settings
               .SetConfiguration("Release")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
               .WithProperty("OutDir", tempFolder.FullName)
        );

    }
}