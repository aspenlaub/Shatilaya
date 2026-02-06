using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("RunTestsOnDebugArtifacts")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class RunTestsOnDebugArtifactsTask : FrostingTask;