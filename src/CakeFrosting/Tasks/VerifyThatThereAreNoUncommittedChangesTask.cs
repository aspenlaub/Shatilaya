using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatThereAreNoUncommittedChanges")]
[TaskDescription("To be described")]
public class VerifyThatThereAreNoUncommittedChangesTask : FrostingTask<ShatilayaContext>;