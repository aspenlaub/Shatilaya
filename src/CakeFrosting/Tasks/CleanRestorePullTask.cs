using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("CleanRestorePull")]
[TaskDescription("Clean, restore packages, pull changes, update nuspec")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(PullTask))]
[IsDependentOn(typeof(RestoreTask))]
public class CleanRestorePullTask : FrostingTask<ShatilayaContext>;