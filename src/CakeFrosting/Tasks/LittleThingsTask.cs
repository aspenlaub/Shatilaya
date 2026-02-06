using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("LittleThings")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class LittleThingsTask : FrostingTask;