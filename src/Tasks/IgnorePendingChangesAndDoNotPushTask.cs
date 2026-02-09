using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("IgnorePendingChangesAndDoNotPush")]
[TaskDescription("Default except check for pending changes and except nuget push")]
[IsDependentOn(typeof(IgnorePendingChangesAndDoNotCreateOrPushPackageTask))]
[IsDependentOn(typeof(CreateNuGetPackageTask))]
public class IgnorePendingChangesAndDoNotPushTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Default except check for pending changes and except nuget push");
    }
}