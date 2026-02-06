using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("UpdateNuspec")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class UpdateNuspecTask : FrostingTask;