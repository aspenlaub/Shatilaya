using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("ReleaseBuild")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class ReleaseBuildTask : FrostingTask;