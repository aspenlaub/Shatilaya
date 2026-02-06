using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatDevelopmentBranchIsAheadOfMaster")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class VerifyThatDevelopmentBranchIsAheadOfMasterTask : FrostingTask;