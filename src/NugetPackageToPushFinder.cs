using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using NuGet.Common;
using NuGet.Protocol;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetPackageToPushFinder : INugetPackageToPushFinder {
        protected IComponentProvider ComponentProvider { get; }

        public NugetPackageToPushFinder(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public async Task<IPackageToPush> FindPackageToPushAsync(IFolder packageFolderWithBinaries, IFolder repositoryFolder, string solutionFileFullName, IErrorsAndInfos errorsAndInfos) {
            IPackageToPush packageToPush = new PackageToPush();
            var projectFileFullName = solutionFileFullName.Replace(".sln", ".csproj");
            if (!File.Exists(projectFileFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Texts.ProjectFileNotFound, projectFileFullName));
                return packageToPush;
            }

            var factory = new ProjectFactory();
            var project = factory.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return packageToPush; }

            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = await ComponentProvider.PeghComponentProvider.SecretRepository.GetAsync(developerSettingsSecret, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return packageToPush; }

            if (developerSettings == null) {
                errorsAndInfos.Errors.Add(string.Format(Texts.MissingDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return packageToPush;
            }

            var feedId = developerSettings.NugetFeedId;
            if (string.IsNullOrEmpty(feedId)) {
                errorsAndInfos.Errors.Add(string.Format(Texts.IncompleteDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return packageToPush;
            }

            var nugetConfigFileFullName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NuGet\nuget.config";
            packageToPush.ApiKey = ComponentProvider.NugetConfigReader.GetApiKey(nugetConfigFileFullName, feedId, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return packageToPush; }

            packageToPush.FeedUrl = developerSettings.NugetFeedUrl;
            if (string.IsNullOrEmpty(packageToPush.FeedUrl)) {
                errorsAndInfos.Errors.Add(string.Format(Texts.IncompleteDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return packageToPush;
            }

            var localPackageRepository = new FindLocalPackagesResourceV2(packageFolderWithBinaries.FullName);
            var localPackages = localPackageRepository.GetPackages(new NullLogger(), CancellationToken.None).Where(p => !p.Identity.Version.IsPrerelease).ToList();
            if (!localPackages.Any()) {
                errorsAndInfos.Errors.Add(string.Format(Texts.NoPackageFilesFound, packageFolderWithBinaries.FullName));
                return packageToPush;
            }

            var latestLocalPackageVersion = localPackages.Max(p => p.Identity.Version.Version);

            var packageId = project.RootNamespace;
            var remotePackages = await ComponentProvider.NugetFeedLister.ListReleasedPackagesAsync(packageToPush.FeedUrl, packageId);
            if (!remotePackages.Any()) {
                errorsAndInfos.Errors.Add(string.Format(Texts.NoRemotePackageFilesFound, packageToPush.FeedUrl, packageId));
                return packageToPush;
            }

            var latestRemotePackageVersion = remotePackages.Max(p => p.Identity.Version.Version);
            if (latestRemotePackageVersion >= latestLocalPackageVersion) { return packageToPush; }

            var remotePackage = remotePackages.First(p => p.Identity.Version.Version == latestRemotePackageVersion);
            if (!string.IsNullOrEmpty(remotePackage.Tags) && repositoryFolder != null) {
                var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(repositoryFolder);
                var tags = remotePackage.Tags.Split(' ');
                if (tags.Contains(headTipIdSha)) { return packageToPush; }
            }

            packageToPush.PackageFileFullName = packageFolderWithBinaries.FullName + @"\" + packageId + "." + latestLocalPackageVersion + ".nupkg";
            if (File.Exists(packageToPush.PackageFileFullName)) { return packageToPush; }

            errorsAndInfos.Errors.Add(string.Format(Texts.FileNotFound, packageToPush.PackageFileFullName));
            return packageToPush;
        }
    }
}
