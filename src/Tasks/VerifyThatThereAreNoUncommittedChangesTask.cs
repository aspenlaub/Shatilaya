using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("VerifyThatThereAreNoUncommittedChanges")]
[TaskDescription("Verify that there are no uncommitted changes")]
public class VerifyThatThereAreNoUncommittedChangesTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Verifying that there are no uncommitted changes");
        var uncommittedErrorsAndInfos = new ErrorsAndInfos();
        context.Container.Resolve<IGitUtilities>().VerifyThatThereAreNoUncommittedChanges(context.RepositoryFolder, uncommittedErrorsAndInfos);
        if (uncommittedErrorsAndInfos.Errors.Any()) {
            throw new Exception(uncommittedErrorsAndInfos.ErrorsToString());
        }
    }
}