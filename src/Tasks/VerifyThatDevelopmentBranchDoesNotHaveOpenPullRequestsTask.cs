using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("VerifyThatDevelopmentBranchDoesNotHaveOpenPullRequests")]
[TaskDescription("Verify that the development branch does not have open pull requests")]
public class VerifyThatDevelopmentBranchDoesNotHaveOpenPullRequestsTask : AsyncFrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.CurrentGitBranch == "master";
    }

    public  override async Task RunAsync(ShatilayaContext context) {
        context.Information("Verifying that the development branch does not have open pull requests");
        var errorsAndInfos = new ErrorsAndInfos();
        bool thereAreOpenPullRequests;
        if (context.SolutionSpecialSettingsDictionary.TryGetValue("PullRequestsToIgnore", out string pullRequestsToIgnore)) {
            thereAreOpenPullRequests = await context.Container.Resolve<IGitHubUtilities>().HasOpenPullRequestAsync(context.RepositoryFolder,
                pullRequestsToIgnore, errorsAndInfos);
        } else {
            thereAreOpenPullRequests = await context.Container.Resolve<IGitHubUtilities>().HasOpenPullRequestAsync(context.RepositoryFolder,
                                                                                                                   errorsAndInfos);
        }
        if (thereAreOpenPullRequests) {
            throw new Exception("There are open pull requests");
        }
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }
}