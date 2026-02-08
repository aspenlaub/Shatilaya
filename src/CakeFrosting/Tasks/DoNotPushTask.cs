using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("DoNotPush")]
[TaskDescription("Default except check nuget push")]
[IsDependentOn(typeof(CleanRestorePullTask))]
[IsDependentOn(typeof(VerifyThatThereAreNoUncommittedChangesTask))]
[IsDependentOn(typeof(VerifyThatDevelopmentBranchIsAheadOfMasterTask))]
[IsDependentOn(typeof(VerifyThatMasterBranchDoesNotHaveOpenPullRequestsTask))]
[IsDependentOn(typeof(VerifyThatDevelopmentBranchDoesNotHaveOpenPullRequestsTask))]
[IsDependentOn(typeof(VerifyThatPullRequestExistsForDevelopmentBranchHeadTipTask))]
[IsDependentOn(typeof(BuildAndTestDebugAndReleaseTask))]
[IsDependentOn(typeof(UpdateNuspecTask))]
[IsDependentOn(typeof(CreateNuGetPackageTask))]
public class DoNotPushTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Default except check nuget push");
    }
}
