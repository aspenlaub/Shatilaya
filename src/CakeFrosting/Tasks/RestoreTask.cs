using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Restore")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class RestoreTask : FrostingTask;