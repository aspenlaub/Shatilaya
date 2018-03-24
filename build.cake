#load "solution.cake"
#addin nuget:?package=Cake.Git
#addin nuget:https://www.aspenlaub.net/nuget/?package=Aspenlaub.Net.GitHub.CSharp.Shatilaya

using Folder = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities.Folder;
using FolderUpdater = Aspenlaub.Net.GitHub.CSharp.Shatilaya.FolderUpdater;
using FolderUpdateMethod = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.FolderUpdateMethod;

masterDebugBinFolder = MakeAbsolute(Directory(masterDebugBinFolder)).FullPath;
masterReleaseBinFolder = MakeAbsolute(Directory(masterReleaseBinFolder)).FullPath;

var target = Argument("target", "Default");
var artifactsFolder = MakeAbsolute(Directory("./artifacts")).FullPath;
var debugArtifactsFolder = MakeAbsolute(Directory("./artifacts/Debug")).FullPath;
var releaseArtifactsFolder = MakeAbsolute(Directory("./artifacts/Release")).FullPath;
var objFolder = MakeAbsolute(Directory("./temp/obj")).FullPath;
var currentGitBranch = GitBranchCurrent(DirectoryPath.FromString("."));
var testResultsFolder = MakeAbsolute(Directory("./TestResults")).FullPath;
var latestBuildCakeUrl = "https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/build.cake?g=" + System.Guid.NewGuid();
var buildCakeFileName = MakeAbsolute(Directory(".")).FullPath + "/build.cake";
var tempFolder = MakeAbsolute(Directory("./temp")).FullPath;
var checkIfBuildCakeIsOutdated = true;
var doDebugCompilation = true;
var doReleaseCompilation = true;

Setup(ctx => { 
  Information("Solution is: " + solution);
  Information("Target is: " + target);
  Information("Artifacts folder is: " + artifactsFolder);
  Information("Current GIT branch is: " + currentGitBranch.FriendlyName);
  Information("Build cake is: " + buildCakeFileName);
  Information("Latest build cake URL is: " + latestBuildCakeUrl);
});

Task("UpdateBuildCake")
  .WithCriteria(() => checkIfBuildCakeIsOutdated)
  .Description("Update build.cake")
  .Does(() => {
    var oldContents = System.IO.File.ReadAllText(buildCakeFileName);
    using (var webClient = new System.Net.WebClient()) {
      webClient.DownloadFile(latestBuildCakeUrl, buildCakeFileName);
    }
    if (oldContents.Replace("\r\n", "\n") != System.IO.File.ReadAllText(buildCakeFileName).Replace("\r\n", "\n")) {
      throw new Exception("Your build.cake file has been updated. Please retry running it.");
    }
  });

Task("Clean")
  .Description("Clean up artifacts and intermediate output folder")
  .Does(() => {
    CleanDirectory(artifactsFolder); 
    CleanDirectory(objFolder); 
  });

Task("Restore")
  .Description("Restore nuget packages")
  .Does(() => {
    NuGetRestore(solution, new NuGetRestoreSettings { ConfigFile = "./src/.nuget/nuget.config" });
  });

Task("DebugBuild")
  .WithCriteria(() => doDebugCompilation)
  .Description("Build solution in Debug and clean up intermediate output folder")
  .Does(() => {
    MSBuild(solution, settings 
      => settings
        .SetConfiguration("Debug")
        .SetVerbosity(Verbosity.Minimal)
		.UseToolVersion(MSBuildToolVersion.NET46)
        .WithProperty("Platform", "Any CPU")
        .WithProperty("OutDir", debugArtifactsFolder)
    );
    CleanDirectory(objFolder); 
  });

Task("RunTestsOnDebugArtifacts")
  .Description("Run unit tests on Debug artifacts")
  .Does(() => {
    MSTest(debugArtifactsFolder + "/*.Test.dll", new MSTestSettings() { NoIsolation = false });
    CleanDirectory(testResultsFolder); 
    DeleteDirectory(testResultsFolder, new DeleteDirectorySettings { Recursive = false, Force = false });
  });

Task("CopyDebugArtifacts")
  .WithCriteria(() => doDebugCompilation && currentGitBranch.FriendlyName == "master")
  .Description("Copy Debug artifacts to master Debug binaries folder")
  .Does(() => {
    var updater = new FolderUpdater();
	var updaterErrorsAndInfos = new ErrorsAndInfos();
    updater.UpdateFolder(new Folder(debugArtifactsFolder.Replace('/', '\\')), new Folder(masterDebugBinFolder.Replace('/', '\\')), 
      FolderUpdateMethod.Assemblies, updaterErrorsAndInfos);
    if (updaterErrorsAndInfos.Errors.Any()) {
	  throw new Exception(string.Join("\r\n", updaterErrorsAndInfos.Errors));
	}
  });

Task("ReleaseBuild")
  .WithCriteria(() => doDebugCompilation && doReleaseCompilation)
  .Description("Build solution in Release and clean up intermediate output folder")
  .Does(() => {
    MSBuild(solution, settings 
      => settings
        .SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
		.UseToolVersion(MSBuildToolVersion.NET46)
        .WithProperty("Platform", "Any CPU")
        .WithProperty("OutDir", releaseArtifactsFolder)
    );
    CleanDirectory(objFolder); 
  });

Task("RunTestsOnReleaseArtifacts")
  .Description("Run unit tests on Release artifacts")
  .Does(() => {
    MSTest(releaseArtifactsFolder + "/*.Test.dll", new MSTestSettings() { NoIsolation = false });
    CleanDirectory(testResultsFolder); 
    DeleteDirectory(testResultsFolder, new DeleteDirectorySettings { Recursive = false, Force = false });
  });

Task("CopyReleaseArtifacts")
  .WithCriteria(() => doDebugCompilation && doReleaseCompilation && currentGitBranch.FriendlyName == "master")
  .Description("Copy Release artifacts to master Release binaries folder")
  .Does(() => {
    var updater = new FolderUpdater();
	var updaterErrorsAndInfos = new ErrorsAndInfos();
    updater.UpdateFolder(new Folder(releaseArtifactsFolder.Replace('/', '\\')), new Folder(masterReleaseBinFolder.Replace('/', '\\')), 
      FolderUpdateMethod.Assemblies, updaterErrorsAndInfos);
    if (updaterErrorsAndInfos.Errors.Any()) {
	  throw new Exception(string.Join("\r\n", updaterErrorsAndInfos.Errors));
	}
  });

Task("Default")
  .IsDependentOn("UpdateBuildCake")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("DebugBuild")
  .IsDependentOn("RunTestsOnDebugArtifacts")
  .IsDependentOn("CopyDebugArtifacts")
  .IsDependentOn("ReleaseBuild")
  .IsDependentOn("RunTestsOnReleaseArtifacts")
  .IsDependentOn("CopyReleaseArtifacts")
  .Does(() => {
  });

RunTarget(target);