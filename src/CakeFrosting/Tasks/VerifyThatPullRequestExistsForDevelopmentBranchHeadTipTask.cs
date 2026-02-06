using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatPullRequestExistsForDevelopmentBranchHeadTip")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class VerifyThatPullRequestExistsForDevelopmentBranchHeadTipTask : FrostingTask;