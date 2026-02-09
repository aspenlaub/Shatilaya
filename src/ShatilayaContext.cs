using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;
using Autofac;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Frosting;

#pragma warning disable CS9107

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya;

public class ShatilayaContext(ICakeContext context) : FrostingContext(context) {
    public IFolder RepositoryFolder => new Folder(GetFolderArgument("repository", GetCurrentFolder().FullName));

    public string Target => GetArgument("target", nameof(DefaultTask).Replace("Task", ""));

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
    public IFolder MasterBinReleaseParentFolder => MasterBinReleaseFolder.ParentFolder();
    public string ReleaseBinHeadTipIdShaFile => MasterBinReleaseParentFolder.FullName + '\\' + "Release.HeadTipSha.txt";

    private readonly DynamicContextProperty<IContainer> _Container = new(nameof(Container));
    public IContainer Container {
        get { return _Container.Get(); }
        set { _Container.Set(value); }
    }

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

    private readonly DynamicContextProperty<bool> _CreateAndPushPackages = new(nameof(CreateAndPushPackages));
    public bool CreateAndPushPackages {
        get { return _CreateAndPushPackages.Get(); }
        set { _CreateAndPushPackages.Set(value); }
    }


    private readonly DynamicContextProperty<bool> _ProduceReleaseCandidate = new(nameof(ProduceReleaseCandidate));
    public bool ProduceReleaseCandidate {
        get { return _ProduceReleaseCandidate.Get(); }
        set { _ProduceReleaseCandidate.Set(value); }
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

    public async Task InitializeAsync() {
        Container = FusionContainerBuilder.CreateContainerUsingFusionNuclideProtchAndGitty("Shatilaya");

        CurrentGitBranch = Container.Resolve<IGitUtilities>().CheckedOutBranch(RepositoryFolder);

        IBranchesWithPackagesRepository branchesWithPackagesRepository = Container.Resolve<IBranchesWithPackagesRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        IList<string> idsOfBranchesWithPackages = branchesWithPackagesRepository.GetBranchIdsAsync(errorsAndInfos).Result;
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
        IsMasterOrBranchWithPackages = CurrentGitBranch == "master" || idsOfBranchesWithPackages.Contains(CurrentGitBranch);

        string fileName = RepositoryFolder.FullName + @"\solution.json";
        if (!File.Exists(fileName)) {
            await SolutionCakeConverter.ConvertSolutionCakeAsync(RepositoryFolder, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) {
                throw new Exception(errorsAndInfos.ErrorsToString());
            }
            if (!File.Exists(fileName)) {
                throw new FileNotFoundException(fileName);
            }
        }
        string contents = await File.ReadAllTextAsync(fileName);
        SolutionSpecialSettingsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(contents);

        bool createAndPushPackages = true;
        if (SolutionSpecialSettingsDictionary.ContainsKey("CreateAndPushPackages")) {
            string createAndPushPackagesText = SolutionSpecialSettingsDictionary["CreateAndPushPackages"].ToUpper();
            if (createAndPushPackagesText != "TRUE" && createAndPushPackagesText != "FALSE") {
                throw new Exception("Setting CreateAndPushPackages must be true or false");
            }
            createAndPushPackages = createAndPushPackagesText == "TRUE";
        }

        CreateAndPushPackages = createAndPushPackages;

        bool produceReleaseCandidate = !createAndPushPackages;
        if (SolutionSpecialSettingsDictionary.ContainsKey("ProduceReleaseCandidate")) {
            string produceReleaseCandidateText = SolutionSpecialSettingsDictionary["ProduceReleaseCandidate"].ToUpper();
            if (produceReleaseCandidateText != "TRUE" && produceReleaseCandidateText != "FALSE") {
                throw new Exception("Setting ProduceReleaseCandidate must be true or false");
            }
            produceReleaseCandidate = produceReleaseCandidateText == "TRUE";
        }

        ProduceReleaseCandidate = produceReleaseCandidate;

        this.Information("Repository folder is: " + RepositoryFolder.FullName);
        this.Information("Solution is: " + SolutionFileFullName);
        this.Information("Solution ID is: " + SolutionId);
        this.Information("Target is: " + Target);
        this.Information("Debug bin folder is: " + DebugBinFolder.FullName);
        this.Information("Release bin folder is: " + ReleaseBinFolder.FullName);
        this.Information("Current GIT branch is: " + CurrentGitBranch);
        this.Information("Is master branch or branch with packages: " + (IsMasterOrBranchWithPackages ? "true" : "false"));
        this.Information("ReleaseCandidate bin folder is: " + MasterReleaseCandidateBinFolder.FullName);
        this.Information("Create and push packages: " + (CreateAndPushPackages ? "true" : "false"));
        this.Information("Produce release candidate: " + (ProduceReleaseCandidate ? "true" : "false"));
    }
}