using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("IgnoreOutdatedBuildCakeAndDoNotPush")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class IgnoreOutdatedBuildCakeAndDoNotPushTask : FrostingTask;