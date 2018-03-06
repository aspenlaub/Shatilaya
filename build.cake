#load "solution.cake"
#addin nuget:?package=Cake.Git

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildFolder = MakeAbsolute(Directory("./artifacts")).FullPath;
var objFolder = MakeAbsolute(Directory("./temp/obj")).FullPath;
var currentGitBranch = GitBranchCurrent(DirectoryPath.FromString("."));
var testResultsFolder = MakeAbsolute(Directory("./TestResults")).FullPath;

Setup(ctx => { 
  Information("Solution is: " + solution);
  Information("Target is: " + target);
  Information("Configuration is: " + configuration);
  Information("BuildFolder is: " + buildFolder);
  Information("Current GIT branch is: " + currentGitBranch.FriendlyName);
});

Task("Clean")
  .Description("Clean up artifacts and intermediate output folder")
  .Does(() => {
    CleanDirectory(buildFolder); 
    CleanDirectory(objFolder); 
  });

Task("Restore")
  .Description("Restore nuget packages")
  .IsDependentOn("Clean")
  .Does(() => {
    NuGetRestore(solution, new NuGetRestoreSettings { ConfigFile = "./src/.nuget/nuget.config" });
  });

Task("Default")
  .IsDependentOn("Restore")
  .Does(() =>
{
});

RunTarget(target);