using System;
using System.IO;
using System.Linq;
using NuGet;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetPackageToPushFinder : INugetPackageToPushFinder {
        protected IComponentProvider ComponentProvider { get; }

        public NugetPackageToPushFinder(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void FindPackageToPush(IFolder packageFolderWithBinaries, IFolder repositoryFolder, string solutionFileFullName, out string packageFileFullName, out string feedUrl, out string apiKey, IErrorsAndInfos errorsAndInfos) {
            packageFileFullName = "";
            feedUrl = "";
            apiKey = "";
            var projectFileFullName = solutionFileFullName.Replace(".sln", ".csproj");
            if (!File.Exists(projectFileFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.ProjectFileNotFound, projectFileFullName));
                return;
            }

            var factory = new ProjectFactory();
            var project = factory.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return; }

            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = ComponentProvider.PeghComponentProvider.SecretRepository.Get(developerSettingsSecret, errorsAndInfos);
            if (developerSettings == null) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.MissingDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return;
            }

            var feedId = developerSettings.NugetFeedId;
            if (string.IsNullOrEmpty(feedId)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.IncompleteDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return;
            }

            var nugetConfigFileFullName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NuGet\nuget.config";
            apiKey = ComponentProvider.NugetConfigReader.GetApiKey(nugetConfigFileFullName, feedId, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) { return; }

            feedUrl = developerSettings.NugetFeedUrl;
            if (string.IsNullOrEmpty(feedUrl)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.IncompleteDeveloperSettings, developerSettingsSecret.Guid + ".xml"));
                return;
            }

            var localPackageRepository = new LocalPackageRepository(packageFolderWithBinaries.FullName);
            var localPackages = localPackageRepository.GetPackages().Where(p => p.IsReleaseVersion()).ToList();
            if (!localPackages.Any()) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.NoPackageFilesFound, packageFolderWithBinaries.FullName));
                return;
            }

            var latestLocalPackageVersion = localPackages.Max(p => p.Version);

            var packageId = project.RootNamespace;
            var remotePackages = ComponentProvider.NugetFeedLister.ListReleasedPackages(feedUrl, packageId);
            if (!remotePackages.Any()) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.NoRemotePackageFilesFound, feedUrl, packageId));
                return;
            }

            var latestRemotePackageVersion = remotePackages.Max(p => p.Version);
            if (latestRemotePackageVersion >= latestLocalPackageVersion) { return; }

            var remotePackage = remotePackages.First(p => p.Version == latestRemotePackageVersion);
            if (!string.IsNullOrEmpty(remotePackage.Tags) && repositoryFolder != null) {
                var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(repositoryFolder);
                var tags = remotePackage.Tags.Split(' ');
                if (tags.Contains(headTipIdSha)) { return; }
            }

            packageFileFullName = packageFolderWithBinaries.FullName + @"\" + packageId + "." + latestLocalPackageVersion + ".nupkg";
            if (File.Exists(packageFileFullName)) { return; }

            errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, packageFileFullName));
        }
    }
}
