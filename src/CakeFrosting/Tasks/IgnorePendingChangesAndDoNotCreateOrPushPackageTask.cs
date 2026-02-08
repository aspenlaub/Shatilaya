using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("IgnorePendingChangesAndDoNotCreateOrPushPackage")]
[TaskDescription("Default except check for pending changes and except nuget create and push")]
[IsDependentOn(typeof(CleanRestorePullTask))]
[IsDependentOn(typeof(BuildAndTestDebugAndReleaseTask))]
[IsDependentOn(typeof(UpdateNuspecTask))]
public class IgnorePendingChangesAndDoNotCreateOrPushPackageTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Default except check for pending changes and except nuget create and push");
    }
}