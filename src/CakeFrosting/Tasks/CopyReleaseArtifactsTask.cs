using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("CopyReleaseArtifacts")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class CopyReleaseArtifactsTask : FrostingTask;