using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("RunTestsOnReleaseArtifacts")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class RunTestsOnReleaseArtifactsTask : FrostingTask;