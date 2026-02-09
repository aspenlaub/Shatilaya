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

[TaskName("CopyDebugArtifacts")]
[TaskDescription("Copy Debug artifacts to master Debug binaries folder")]
public class CopyDebugArtifactsTask : AsyncFrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.IsMasterOrBranchWithPackages;
    }

    public override async Task RunAsync(ShatilayaContext context) {
        context.Information("Copying Debug artifacts to master Debug binaries folder");
        IFolderUpdater updater = context.Container.Resolve<IFolderUpdater>();
        var updaterErrorsAndInfos = new ErrorsAndInfos();
        string headTipIdSha = context.Container.Resolve<IGitUtilities>().HeadTipIdSha(context.RepositoryFolder);
        if (!File.Exists(context.ReleaseBinHeadTipIdShaFile)) {
            updater.UpdateFolder(context.DebugBinFolder, context.MasterBinDebugFolder,
                FolderUpdateMethod.AssembliesButNotIfOnlySlightlyChanged,
                "Aspenlaub.Net.GitHub.CSharp." + context.SolutionId, updaterErrorsAndInfos);
        } else {
            await updater.UpdateFolderAsync(context.SolutionId, context.CurrentGitBranch, headTipIdSha,
                context.DebugBinFolder, await File.ReadAllTextAsync(context.ReleaseBinHeadTipIdShaFile), context.MasterBinDebugFolder,
                false, context.CreateAndPushPackages, context.MainNugetFeedId, updaterErrorsAndInfos);
        }
        updaterErrorsAndInfos.Infos.ToList().ForEach(context.Information);
        if (updaterErrorsAndInfos.Errors.Any()) {
            throw new Exception(updaterErrorsAndInfos.ErrorsToString());
        }
    }
}