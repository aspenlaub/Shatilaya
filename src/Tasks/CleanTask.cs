using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("Clean")]
[TaskDescription("Clean up artifacts and intermediate output folder")]
[IsDependentOn(typeof(InitializeContextTask))]
public class CleanTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Log.Information($"Cleaning {context.DebugBinFolder.FullName}");
        context.CleanDirectory(context.DebugBinFolder.FullName);
        context.Log.Information($"Cleaning {context.ReleaseBinFolder.FullName}");
        context.CleanDirectory(context.ReleaseBinFolder.FullName);
    }
}