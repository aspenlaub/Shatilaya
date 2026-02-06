using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("CreateNuGetPackage")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class CreateNuGetPackageTask : FrostingTask;