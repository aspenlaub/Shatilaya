using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("IgnorePendingChanges")]
[TaskDescription("Default except check for pending changes")]
[IsDependentOn(typeof(IgnorePendingChangesAndDoNotPushTask))]
[IsDependentOn(typeof(PushNuGetPackageTask))]
[IsDependentOn(typeof(CleanObjectFoldersTask))]
public class IgnorePendingChangesTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Default except check for pending changes");
    }
}