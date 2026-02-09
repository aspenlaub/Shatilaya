using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
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

        context.Container.Resolve<IGitUtilities>().Pull(context.RepositoryFolder, developerSettings.Author, developerSettings.Email);
    }
}