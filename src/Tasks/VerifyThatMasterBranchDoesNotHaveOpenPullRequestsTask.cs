using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("VerifyThatMasterBranchDoesNotHaveOpenPullRequests")]
[TaskDescription("Verify that the master branch does not have open pull requests")]
public class VerifyThatMasterBranchDoesNotHaveOpenPullRequestsTask : AsyncFrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.CurrentGitBranch == "master";
    }

    public override async Task RunAsync(ShatilayaContext context) {
        context.Information("Verifying that the master branch does not have open pull requests");
        var errorsAndInfos = new ErrorsAndInfos();
        bool thereAreOpenPullRequests = false;

        await context.OnlineLogic.ExecuteOnlineActionWithRetriesAsync(async _ => {
            thereAreOpenPullRequests = await TryCheckingIfMasterBranchHasOpenPullRequests(context, errorsAndInfos);
        }, "Checking if there are open rull requests", errorsAndInfos);

        errorsAndInfos.Infos.ToList().ForEach(context.Information);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }

        if (thereAreOpenPullRequests) {
            throw new Exception("There are open pull requests");
        }
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }

    private static async Task<bool> TryCheckingIfMasterBranchHasOpenPullRequests(ShatilayaContext context, IErrorsAndInfos errorsAndInfos) {
        bool thereAreOpenPullRequests;
        if (context.SolutionSpecialSettingsDictionary.TryGetValue("PullRequestsToIgnore", out string pullRequestsToIgnore)) {
            thereAreOpenPullRequests = await context.Container.Resolve<IGitHubUtilities>()
                .HasOpenPullRequestAsync(context.RepositoryFolder, pullRequestsToIgnore, errorsAndInfos);
        } else {
            thereAreOpenPullRequests = await context.Container.Resolve<IGitHubUtilities>()
                .HasOpenPullRequestAsync(context.RepositoryFolder, errorsAndInfos);
        }

        return thereAreOpenPullRequests;
    }
}