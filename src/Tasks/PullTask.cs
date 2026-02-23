using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("Pull")]
[TaskDescription("Pull latest changes")]
public class PullTask : AsyncFrostingTask<ShatilayaContext> {
    public override async Task RunAsync(ShatilayaContext context) {
        context.Log.Information($"Pulling {context.RepositoryFolder.FullName}");
        var developerSettingsSecret = new DeveloperSettingsSecret();
        var errorsAndInfos = new ErrorsAndInfos();
        DeveloperSettings developerSettings = await context.Container.Resolve<ISecretRepository>().GetAsync(developerSettingsSecret, errorsAndInfos);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }

        await context.OnlineLogic.ExecuteOnlineActionWithRetriesAsync(_ => TryPullAsync(context, developerSettings),
            "Pulling latest changes from remote", errorsAndInfos);
        errorsAndInfos.Infos.ToList().ForEach(context.Information);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }

    private static async Task TryPullAsync(ShatilayaContext context, DeveloperSettings developerSettings) {
        context.Container.Resolve<IGitUtilities>().Pull(context.RepositoryFolder, developerSettings.Author, developerSettings.Email);
        await Task.CompletedTask;
    }
}