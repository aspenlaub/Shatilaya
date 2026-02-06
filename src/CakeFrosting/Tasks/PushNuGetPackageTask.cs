using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("PushNuGetPackage")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class PushNuGetPackageTask : FrostingTask;