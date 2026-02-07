using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Frosting;

#pragma warning disable CS9107

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

public class ShatilayaContext(ICakeContext context) : FrostingContext(context) {
    public IFolder RepositoryFolder => new Folder(GetFolderArgument("repository", GetCurrentFolder().FullName));

    public string Target => GetArgument("target", "Default");

    public string SolutionFileFullName => GetSolutionFileFullName();

    public string SolutionId => SolutionFileFullName[(SolutionFileFullName.LastIndexOf('\\') + 1)..].Replace(".slnx", "");
    public IFolder DebugBinFolder => RepositoryFolder.SubFolder(@"src\bin\Debug");
    public IFolder ReleaseBinFolder => RepositoryFolder.SubFolder(@"src\bin\Release");
    public IFolder TestResultsFolder => RepositoryFolder.SubFolder("TestResults");
    public IFolder TempFolder => RepositoryFolder.SubFolder("temp");

    public string MainNugetFeedId = NugetFeed.AspenlaubLocalFeed;

    public IFolder MasterBinDebugFolder => RepositoryFolder.ParentFolder().SubFolder(SolutionId + "Bin").SubFolder("Debug");
    public IFolder MasterBinReleaseFolder => RepositoryFolder.ParentFolder().SubFolder(SolutionId + "Bin").SubFolder("Release");
    public IFolder MasterReleaseCandidateBinFolder => MasterBinReleaseFolder.ParentFolder().SubFolder("ReleaseCandidate");

    private readonly DynamicContextProperty<string> _CurrentGitBranch = new(nameof(CurrentGitBranch));
    public string CurrentGitBranch {
        get { return _CurrentGitBranch.Get(); }
        set { _CurrentGitBranch.Set(value); }
    }

    private readonly DynamicContextProperty<bool> _IsMasterOrBranchWithPackages = new(nameof(IsMasterOrBranchWithPackages));
    public bool IsMasterOrBranchWithPackages {
        get { return _IsMasterOrBranchWithPackages.Get(); }
        set { _IsMasterOrBranchWithPackages.Set(value); }
    }

    private readonly DynamicContextProperty<Dictionary<string, string>> _SolutionSpecialSettingsDictionary = new(nameof(SolutionSpecialSettingsDictionary));
    public Dictionary<string, string> SolutionSpecialSettingsDictionary {
        get { return _SolutionSpecialSettingsDictionary.Get(); }
        set { _SolutionSpecialSettingsDictionary.Set(value); }
    }

    private string GetFolderArgument(string argumentName, string defaultValue) {
        return GetArgument(argumentName, defaultValue);
    }

    private string GetArgument(string argumentName, string defaultValue) {
        string argumentValue = context.Argument(argumentName, defaultValue);
        return argumentValue;
    }

    private Folder GetCurrentFolder() {
        return new Folder(context.MakeAbsolute(context.Directory(".")).FullPath.Replace("/", "\\"));
    }

    private string GetSolutionFileFullName() {
        IFolder folder = RepositoryFolder.SubFolder("src");
        var solutionFiles = Directory.GetFiles(folder.FullName, "*.slnx").ToList();
        return solutionFiles.Count == 1
            ? solutionFiles[0]
            : throw new ArgumentException($"Solution file missing or not unique in {folder.FullName}");
    }

}