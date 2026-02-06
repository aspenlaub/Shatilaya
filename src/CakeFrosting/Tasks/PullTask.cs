using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Pull")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class PullTask : FrostingTask;