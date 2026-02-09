using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Push;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("PushNuGetPackage")]
[TaskDescription("Push nuget package")]
public class PushNuGetPackageTask : AsyncFrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.IsMasterOrBranchWithPackages && context.CreateAndPushPackages;
    }

    public override async Task RunAsync(ShatilayaContext context) {
        context.Information("Pushing nuget package");
        INugetPackageToPushFinder nugetPackageToPushFinder = context.Container.Resolve<INugetPackageToPushFinder>();
        var errorsAndInfos = new ErrorsAndInfos();
        IPackageToPush packageToPush = await nugetPackageToPushFinder.FindPackageToPushAsync(context.MainNugetFeedId,
             context.MasterBinReleaseFolder, context.RepositoryFolder, context.SolutionFileFullName,
             context.CurrentGitBranch, errorsAndInfos);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
        string headTipSha = context.Container.Resolve<IGitUtilities>().HeadTipIdSha(context.RepositoryFolder);
        if (packageToPush != null && !string.IsNullOrEmpty(packageToPush.PackageFileFullName) && !string.IsNullOrEmpty(packageToPush.FeedUrl)) {
            errorsAndInfos.Infos.ToList().ForEach(context.Information);
            context.Information("Pushing " + packageToPush.PackageFileFullName + " to " + packageToPush.FeedUrl + "..");
            context.NuGetPush(packageToPush.PackageFileFullName, new NuGetPushSettings { Source = packageToPush.FeedUrl });
        } else {
            context.Information("Did not find any package to push, adding " + headTipSha + " to pushed headTipShas for " + context.MainNugetFeedId);
        }
        IPushedHeadTipShaRepository pushedHeadTipShaRepository = context.Container.Resolve<IPushedHeadTipShaRepository>();
        errorsAndInfos = new ErrorsAndInfos();
        if (packageToPush != null && !string.IsNullOrEmpty(packageToPush.Id) && !string.IsNullOrEmpty(packageToPush.Version)) {
            await pushedHeadTipShaRepository.AddAsync(context.MainNugetFeedId, headTipSha, packageToPush.Id, packageToPush.Version, errorsAndInfos);
        } else {
            await pushedHeadTipShaRepository.AddAsync(context.MainNugetFeedId, headTipSha, errorsAndInfos);
        }
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }
}