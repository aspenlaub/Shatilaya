using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("VerifyThatThereAreUncommittedChanges")]
[TaskDescription("Verify that there are uncommitted changes")]
public class VerifyThatThereAreUncommittedChangesTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Verifying that there are uncommitted changes");
        var errorsAndInfos = new ErrorsAndInfos();
        context.Container.Resolve<IGitUtilities>().VerifyThatThereAreNoUncommittedChanges(context.RepositoryFolder, errorsAndInfos);
        if (!errorsAndInfos.Errors.Any()) {
            throw new Exception("The check for uncommitted changes did not fail, this is unexpected");
        }
    }
}