using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("VerifyThatPullRequestExistsForDevelopmentBranchHeadTip")]
[TaskDescription("Verify that the master branch does have a pull request for the checked out development branch head tip")]
public class VerifyThatPullRequestExistsForDevelopmentBranchHeadTipTask : AsyncFrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return !context.IsMasterOrBranchWithPackages;

    }

    public override async Task RunAsync(ShatilayaContext context) {
        context.Information("Verifying that the master branch does have a pull request for the checked out development branch head tip");
        var errorsAndInfos = new ErrorsAndInfos();
        bool thereArePullRequests = await context.Container.Resolve<IGitHubUtilities>().HasPullRequestForThisBranchAndItsHeadTipAsync(context.RepositoryFolder, errorsAndInfos);
        if (!thereArePullRequests) {
            throw new Exception("There is no pull request for this development branch and its head tip");
        }
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }
}