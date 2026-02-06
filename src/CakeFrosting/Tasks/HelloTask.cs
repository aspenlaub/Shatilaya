using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

[TaskName("Hello")]
public sealed class HelloTask : FrostingTask<BuildContext> {
    public override void Run(BuildContext context) {
        context.Log.Information("Hello");
    }
}