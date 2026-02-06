using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatThereAreUncommittedChanges")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class VerifyThatThereAreUncommittedChangesTask : FrostingTask;