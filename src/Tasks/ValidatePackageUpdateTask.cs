using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("ValidatePackageUpdate")]
[TaskDescription("Build and test debug and release, update nuspec")]
[IsDependentOn(typeof(CleanRestorePullTask))]
[IsDependentOn(typeof(VerifyThatThereAreUncommittedChangesTask))]
[IsDependentOn(typeof(BuildAndTestDebugAndReleaseTask))]
[IsDependentOn(typeof(UpdateNuspecTask))]
public class ValidatePackageUpdateTask : FrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.IsMasterOrBranchWithPackages;
    }
}