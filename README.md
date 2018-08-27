# Shatilaya

Summary: this repository contains my automation and continuous integration tools

## What does *Shatilaya* mean?

Shatilaya is the vulcan word for automation

## Can it be useful to you?

Yes, if you are working with Visual Studio 2017 (Visual Studio 2015 projects probably still work, but I am moving away from them). How you get started is illustrated in the [Roxann](https://github.com/aspenlaub/Roxann) repository's readme file

## Built around build.cake

The whole Shatilaya project is built around the ```build.cake``` script. Here are the tasks implemented in ```build.cake``` as they were when this readme file was updated:

### UpdateBuildCake
All my projects use the same ```build.cake```, and they should always use the latest version published in the Shatilaya repository. Cake task ```UpdateBuildCake``` updates the file if it is outdated. If that is the case for e.g. repository Chab, I will commit the changed  ```build.cake``` to the Chab repository with comment "Build.cake update"

### Clean
As the name says, this step cleans some folders

### Restore
Restore nuget packages for the solution

### UpdateNuspec
Create or update a nuspec file that will help with the creation of a nuget package. Currently, all dll and pdb files are packages, except for the test assemblies, of course

### VerifyThatThereAreNoUncommittedChanges
This step is necessary for me because I later tag the nuget package with the commit unique identification, so that running ```build.cake``` again will result in another package file, but that will not be pushed to my nuget feed.

### VerifyThatDevelopmentBranchIsAheadOfMaster
Running ```build.cake``` for a development branch is only necessary if the branch is ahead of the master

### VerifyThatMasterBranchDoesNotHaveOpenPullRequests
Stop the master branch build if there are open pull requests

### DebugBuild
Build the solution, results are in ```artifacts\Debug```

### RunTestsOnDebugArtifacts
Run all tests in ```artifacts\Debug``` test assemblies. MSTest is used if available, otherwise VSTest

### CopyDebugArtifacts
Copy ```artifacts\Debug``` files to the masterDebugBinFolder as specified in the ```solution.cake``` file

### ReleaseBuild
Build the solution, results are in ```artifacts\Release```

### RunTestsOnReleaseArtifacts
Run all tests in ```artifacts\Release``` test assemblies. MSTest is used if available, otherwise VSTest

### CopyReleaseArtifacts
Copy ```artifacts\Release``` files to the masterReleaseBinFolder as specified in the ```solution.cake``` file

### CreateNuGetPackage
Create a nuget package in the masterReleaseBinFolder as specified in the ```solution.cake``` file

### PushNuGetPackage
Push the nuget package to the nuget feed as specified in your secret repository

## NuGet feed

https://www.aspenlaub.net/nuget
