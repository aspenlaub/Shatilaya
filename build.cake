#load "solution.cake"
#addin nuget:?package=Cake.Git

var target = Argument("target", "Default");
var buildFolder = MakeAbsolute(Directory("./artifacts")).FullPath;
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
  Information("BuildFolder is: " + buildFolder);
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
  .IsDependentOn("UpdateBuildCake")
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

Task("DebugBuild")
  .WithCriteria(() => doDebugCompilation)
  .Description("Debug build solution and clean up intermediate output folder")
  .IsDependentOn("Restore")
  .Does(() => {
    MSBuild(solution, settings 
      => settings
        .SetConfiguration("Debug")
        .SetVerbosity(Verbosity.Minimal)
		.UseToolVersion(MSBuildToolVersion.NET46)
        .WithProperty("Platform", "Any CPU")
        .WithProperty("OutDir", buildFolder));
    CleanDirectory(objFolder); 
  });

Task("Default")
  .IsDependentOn("DebugBuild")
  .Does(() =>
{
});

RunTarget(target);