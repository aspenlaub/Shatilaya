using System.Threading.Tasks;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("InitializeContext")]
[TaskDescription("Înitializing context")]
public class InitializeContextTask : AsyncFrostingTask<ShatilayaContext> {
    public override async Task RunAsync(ShatilayaContext context) {
        context.Log.Information("Initializing context");
        await context.InitializeAsync();
    }
}