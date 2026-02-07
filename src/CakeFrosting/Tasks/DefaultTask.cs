using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Default")]
[IsDependentOn(typeof(LittleThingsTask))]
[IsDependentOn(typeof(BuildAndTestDebugAndReleaseTask))]
[IsDependentOn(typeof(UpdateNuspecTask))]
[IsDependentOn(typeof(CreateNuGetPackageTask))]
[IsDependentOn(typeof(PushNuGetPackageTask))]
[IsDependentOn(typeof(CleanObjectFoldersTask))]
public class DefaultTask : FrostingTask<ShatilayaContext>;