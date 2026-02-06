using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("IgnoreOutdatedBuildCakePendingChangesAndDoCreateOrPushPackage")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class IgnoreOutdatedBuildCakePendingChangesAndDoCreateOrPushPackageTask : FrostingTask;