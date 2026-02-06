using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Clean")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class CleanTask : FrostingTask;