using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("UpdateBuildCake")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class UpdateBuildCakeTask : FrostingTask;