using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("IgnoreOutdatedBuildCakePendingChanges")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class IgnoreOutdatedBuildCakePendingChangesTask : FrostingTask;