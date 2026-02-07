using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatMasterBranchDoesNotHaveOpenPullRequests")]
[TaskDescription("To be described")]
public class VerifyThatMasterBranchDoesNotHaveOpenPullRequestsTask : FrostingTask<ShatilayaContext>;