using System;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("VerifyThatDevelopmentBranchIsAheadOfMaster")]
[TaskDescription("Verify that the development branch is at least one commit after the master")]
public class VerifyThatDevelopmentBranchIsAheadOfMasterTask : FrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return !context.IsMasterOrBranchWithPackages;
    }

    public override void Run(ShatilayaContext context) {
        context.Information("Verifying that the development branch is at least one commit after the master");
        if (!context.Container.Resolve<IGitUtilities>().IsBranchAheadOfMaster(context.RepositoryFolder)) {
            throw new Exception("Branch must be at least one commit ahead of the origin/master");
        }
    }
}