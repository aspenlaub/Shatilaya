using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Pull")]
[TaskDescription("Pull latest changes")]
public class PullTask : AsyncFrostingTask<ShatilayaContext> {
    public override async Task RunAsync(ShatilayaContext context) {
        context.Log.Information($"Pulling {context.RepositoryFolder.FullName}");
        var developerSettingsSecret = new DeveloperSettingsSecret();
        var pullErrorsAndInfos = new ErrorsAndInfos();
        IContainer container = FusionContainerBuilder.CreateContainerUsingFusionNuclideProtchAndGitty("Shatilaya");
        DeveloperSettings developerSettings = await container.Resolve<ISecretRepository>().GetAsync(developerSettingsSecret, pullErrorsAndInfos);
        if (pullErrorsAndInfos.Errors.Any()) {
            throw new Exception(pullErrorsAndInfos.ErrorsToString());
        }

        container.Resolve<IGitUtilities>().Pull(context.RepositoryFolder, developerSettings.Author, developerSettings.Email);
    }
}