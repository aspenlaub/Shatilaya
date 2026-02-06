using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("CleanObjectFolders")]
[TaskDescription("To be described")]
[IsDependentOn(typeof(WorldTask))]
public class CleanObjectFoldersTask : FrostingTask;