using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("CleanRestorePull")]
[TaskDescription("Clean, restore packages, pull changes")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(PullTask))]
[IsDependentOn(typeof(RestoreTask))]
public class CleanRestorePullTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Cleaning, restoring packages, pulling changes");
    }
}