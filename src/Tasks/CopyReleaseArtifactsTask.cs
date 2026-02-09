using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("CopyReleaseArtifacts")]
[TaskDescription("Copy Release artifacts to master Release binaries folder")]
public class CopyReleaseArtifactsTask : AsyncFrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.IsMasterOrBranchWithPackages;
    }

    public override async Task RunAsync(ShatilayaContext context) {
        context.Information("Copying Release artifacts to master Release binaries folder");
        IFolderUpdater updater = context.Container.Resolve<IFolderUpdater>();
        var updaterErrorsAndInfos = new ErrorsAndInfos();
        string headTipIdSha = context.Container.Resolve<IGitUtilities>().HeadTipIdSha(context.RepositoryFolder);
        if (!File.Exists(context.ReleaseBinHeadTipIdShaFile)) {
            updater.UpdateFolder(context.ReleaseBinFolder, context.MasterBinReleaseFolder,
                FolderUpdateMethod.AssembliesButNotIfOnlySlightlyChanged,
                "Aspenlaub.Net.GitHub.CSharp." + context.SolutionId, updaterErrorsAndInfos);
        } else {
            await updater.UpdateFolderAsync(context.SolutionId, context.CurrentGitBranch, headTipIdSha,
                context.ReleaseBinFolder, await File.ReadAllTextAsync(context.ReleaseBinHeadTipIdShaFile), context.MasterBinReleaseFolder,
                false, context.CreateAndPushPackages, context.MainNugetFeedId, updaterErrorsAndInfos);
        }
        updaterErrorsAndInfos.Infos.ToList().ForEach(context.Information);
        if (updaterErrorsAndInfos.Errors.Any()) {
            throw new Exception(updaterErrorsAndInfos.ErrorsToString());
        }
    }
}