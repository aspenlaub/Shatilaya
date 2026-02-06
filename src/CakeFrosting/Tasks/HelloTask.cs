using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Hello")]
[TaskDescription("To be described")]
public sealed class HelloTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Log.Information("Hello");
    }
}