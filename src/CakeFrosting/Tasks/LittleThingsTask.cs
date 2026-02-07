using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("LittleThings")]
[TaskDescription("Default but do not build or test in debug or release, and do not create or push nuget package")]
[IsDependentOn(typeof(CleanRestorePullTask))]
[IsDependentOn(typeof(VerifyThatThereAreNoUncommittedChangesTask))]
[IsDependentOn(typeof(VerifyThatDevelopmentBranchIsAheadOfMasterTask))]
[IsDependentOn(typeof(VerifyThatMasterBranchDoesNotHaveOpenPullRequestsTask))]
[IsDependentOn(typeof(VerifyThatDevelopmentBranchDoesNotHaveOpenPullRequestsTask))]
[IsDependentOn(typeof(VerifyThatPullRequestExistsForDevelopmentBranchHeadTipTask))]
public class LittleThingsTask : FrostingTask<ShatilayaContext>;