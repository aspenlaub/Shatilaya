using System.Threading.Tasks;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("InitializeContextForMsBuild")]
[TaskDescription("Înitializing context for MSBuild")]
public class InitializeContextForMsBuildTask : AsyncFrostingTask<ShatilayaContext> {
    public override async Task RunAsync(ShatilayaContext context) {
        context.Log.Information("Initializing context for MSBuild");
        await context.InitializeForMsBuildAsync();
    }
}
