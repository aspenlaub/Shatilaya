#load "solution.cake"
#addin nuget:?package=Cake.Git

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildFolder = MakeAbsolute(Directory("./artifacts")).FullPath;
var objFolder = MakeAbsolute(Directory("./temp/obj")).FullPath;
var currentGitBranch = GitBranchCurrent(DirectoryPath.FromString("."));
var testResultsFolder = MakeAbsolute(Directory("./TestResults")).FullPath;
var latestBuildCakeUrl = "https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/build.cake";
var buildCakeFileName = MakeAbsolute(Directory(".")).FullPath + "/build.cake";
var checkIfBuildCakeIsOutdated = true;

Setup(ctx => { 
  Information("Solution is: " + solution);
  Information("Target is: " + target);
  Information("Configuration is: " + configuration);
  Information("BuildFolder is: " + buildFolder);
  Information("Current GIT branch is: " + currentGitBranch.FriendlyName);
  Information("Build cake is: " + buildCakeFileName);
});

Task("UpdateBuildCake")
  .Description("Update build.cake")
  .Does(() => {
    if (checkIfBuildCakeIsOutdated) {
      var oldContents = System.IO.File.ReadAllText(buildCakeFileName);
      var request = System.Net.WebRequest.Create(latestBuildCakeUrl) as System.Net.HttpWebRequest;
	  string newContents;
      using (var response = (System.Net.HttpWebResponse)request.GetResponse()) {
        var stream = response.GetResponseStream();
        using (var reader = new StreamReader(stream, Encoding.Default)) {
          newContents = reader.ReadToEnd();
		}
      }
      if (oldContents.Replace("\r\n", "\n") != newContents.Replace("\r\n", "\n")) {
	    System.IO.File.WriteAllText(buildCakeFileName, newContents);
	    throw new Exception("Your build.cake file has been updated. Please retry running it.");
      }
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

Task("Default")
  .IsDependentOn("Restore")
  .Does(() =>
{
});

RunTarget(target);