using System;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

#pragma warning disable CS9107

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

public class ShatilayaContext(ICakeContext context) : FrostingContext(context) {
    public IFolder MasterBinDebugFolder => GetFolderArgument("masterDebugBinFolder");
    public IFolder MasterBinReleaseFolder => GetFolderArgument("masterReleaseBinFolder");
    public IFolder MasterReleaseCandidateBinFolder => MasterBinReleaseFolder.ParentFolder().SubFolder("ReleaseCandidate");

    public string Target => GetArgument("target", "Default");

    public bool IsMasterOrBranchWithPackages { get; set; }

    public string SolutionFileFullName => GetArgument("solution");
    public string SolutionId => SolutionFileFullName.Substring(SolutionFileFullName.LastIndexOf('/') + 1).Replace(".slnx", "");
    public IFolder DebugBinFolder => GetRelativeFolder("./src/bin/Debug");
    public IFolder ReleaseBinFolder => GetRelativeFolder("./src/bin/Release");
    public IFolder TestResultsFolder => GetRelativeFolder("./TestResults");
    public IFolder TempFolder => GetRelativeFolder("./temp");
    public IFolder RepositoryFolder => GetRelativeFolder(".");

    public string BuildCakeFileName => GetRelativeFolder(".").FullName + "/build.cake";
    public string TempCakeBuildFileName => TempFolder + "/build.cake.new";

    public string MainNugetFeedId = NugetFeed.AspenlaubLocalFeed;

    public IContainer Container = FusionContainerBuilder.CreateContainerUsingFusionNuclideProtchAndGitty("Shatilaya");

    private Folder GetFolderArgument(string argumentName) {
        string argumentValue = GetArgument(argumentName);
        return new Folder(context.MakeAbsolute(context.Directory(argumentValue)).FullPath);
    }

    private string GetArgument(string argumentName) {
        string argumentValue = GetArgument(argumentName, "");
        return string.IsNullOrEmpty(argumentValue)
            ? throw new ArgumentException(argumentName)
            : argumentValue;
    }

    private string GetArgument(string argumentName, string defaultValue) {
        string argumentValue = context.Argument(argumentName, defaultValue);
        return argumentValue;
    }

    private Folder GetRelativeFolder(string relativeFolder) {
        return new Folder(context.MakeAbsolute(context.Directory(relativeFolder)).FullPath);
    }
}