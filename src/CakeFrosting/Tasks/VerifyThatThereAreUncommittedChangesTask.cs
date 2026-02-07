using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatThereAreUncommittedChanges")]
[TaskDescription("Verify that there are uncommitted changes")]
public class VerifyThatThereAreUncommittedChangesTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Verifying that there are uncommitted changes");
        var uncommittedErrorsAndInfos = new ErrorsAndInfos();
        IContainer container = FusionContainerBuilder.CreateContainerUsingFusionNuclideProtchAndGitty("Shatilaya");
        container.Resolve<IGitUtilities>().VerifyThatThereAreNoUncommittedChanges(context.RepositoryFolder, uncommittedErrorsAndInfos);
        if (!uncommittedErrorsAndInfos.Errors.Any()) {
            throw new Exception("The check for uncommitted changes did not fail, this is unexpected");
        }
    }
}