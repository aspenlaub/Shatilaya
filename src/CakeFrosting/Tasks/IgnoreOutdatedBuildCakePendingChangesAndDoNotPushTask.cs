using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("IgnoreOutdatedBuildCakePendingChangesAndDoNotPush")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class IgnoreOutdatedBuildCakePendingChangesAndDoNotPushTask : FrostingTask;