using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("CleanRestorePullUpdateBuildCake")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class CleanRestorePullUpdateBuildCakeTask : FrostingTask;