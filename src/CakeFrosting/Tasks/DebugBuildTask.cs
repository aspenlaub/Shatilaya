using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("DebugBuild")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class DebugBuildTask : FrostingTask;