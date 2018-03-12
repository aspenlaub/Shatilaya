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
var objFolder = MakeAbsolute(Directory("./temp/obj")).FullPath;
var currentGitBranch = GitBranchCurrent(DirectoryPath.FromString("."));
var testResultsFolder = MakeAbsolute(Directory("./TestResults")).FullPath;
var latestBuildCakeUrl = "https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/build.cake?g=" + System.Guid.NewGuid();
var buildCakeFileName = MakeAbsolute(Directory(".")).FullPath + "/build.cake";
var checkIfBuildCakeIsOutdated = true;
var doDebugCompilation = true;

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
  .Description("Debug build solution and clean up intermediate output folder")
  .Does(() => {
    MSBuild(solution, settings 
      => settings
        .SetConfiguration("Debug")
        .SetVerbosity(Verbosity.Minimal)
		.UseToolVersion(MSBuildToolVersion.NET46)
        .WithProperty("Platform", "Any CPU")
        .WithProperty("OutDir", artifactsFolder));
    CleanDirectory(objFolder); 
  })
  .OnError(() => {
    throw new Exception("Debug build failed");
  });

Task("CopyDebugArtifacts")
  .WithCriteria(() => doDebugCompilation && currentGitBranch.FriendlyName == "master")
  .Description("Debug build solution and clean up intermediate output folder")
  .Does(() => {
    var updater = new FolderUpdater();
	updater.UpdateFolder(new Folder(artifactsFolder.Replace('/', '\\')), new Folder(masterDebugBinFolder.Replace('/', '\\')), FolderUpdateMethod.Assemblies);
  })
  .OnError(() => {
    throw new Exception("Copying of debug artifacts failed");
  });

Task("Default")
  .IsDependentOn("UpdateBuildCake")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("DebugBuild")
  .IsDependentOn("CopyDebugArtifacts")
  .Does(() => {
  });

RunTarget(target);