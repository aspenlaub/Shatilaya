using System.Threading.Tasks;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("World")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(HelloTask))]
public sealed class WorldTask : AsyncFrostingTask<ShatilayaContext> {
    // Tasks can be asynchronous
    public override async Task RunAsync(ShatilayaContext context) {
        context.Log.Information("World");
        await Task.CompletedTask;
    }
}