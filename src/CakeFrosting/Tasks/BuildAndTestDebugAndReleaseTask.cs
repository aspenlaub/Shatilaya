using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("BuildAndTestDebugAndRelease")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class BuildAndTestDebugAndReleaseTask : FrostingTask;