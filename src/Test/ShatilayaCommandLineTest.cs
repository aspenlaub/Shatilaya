using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IProcessRunner = Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces.IProcessRunner;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class ShatilayaCommandLineTest : ShatilayaTestBase {
    [TestMethod]
    public void CanCleanRestorePull() {
        PutTogetherRunnerArguments("CleanRestorePull", out string executableFullName, out string arguments, out Folder workingFolder);
        IProcessRunner processRunner = Container.Resolve<IProcessRunner>();
        var errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        List<string> expectedInfos = [
          "Cleaning", "Restoring", "Pulling"
        ];
        foreach (string expectedInfo in expectedInfos) {
            Assert.Contains(x => x.StartsWith(expectedInfo), errorsAndInfos.Infos, $"No info starts with {expectedInfo}");
        }
    }

    [TestMethod]
    public void CanDoLittleThings() {
        PutTogetherRunnerArguments("LittleThings", out string executableFullName, out string arguments, out Folder workingFolder);
        IProcessRunner processRunner = Container.Resolve<IProcessRunner>();
        var errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        List<string> expectedInfos = [
            "Verifying that the master branch", "Verifying that there are no uncommitted"
        ];
        foreach (string expectedInfo in expectedInfos) {
            Assert.Contains(x => x.StartsWith(expectedInfo), errorsAndInfos.Infos, $"No info starts with {expectedInfo}");
        }
    }

    [TestMethod]
    public void CanBuildTestAndCopy() {
        PutTogetherRunnerArguments("LittleThings", out string executableFullName, out string arguments, out Folder workingFolder);
        IProcessRunner processRunner = Container.Resolve<IProcessRunner>();
        var errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());

        PutTogetherRunnerArguments("BuildAndTestDebugAndRelease", out executableFullName, out arguments, out workingFolder);
        errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());

        List<string> expectedInfos = [
            "Building solution in Debug",
            "Running unit and integration tests on Debug artifacts",
            "Running tests in",
            "Passed!",
            "Copying Debug artifacts to master Debug binaries folder",
            "Building solution in Release",
            "Running unit and integration tests on Release artifacts",
            "Copying Release artifacts to master Release binaries folder"
        ];
        foreach (string expectedInfo in expectedInfos) {
            Assert.Contains(x => x.StartsWith(expectedInfo), errorsAndInfos.Infos, $"No info starts with {expectedInfo}");
        }
    }

    private void PutTogetherRunnerArguments(string target, out string executableFullName, out string arguments, out Folder workingFolder) {
        ShatilayaFinder.FindShatilaya(out executableFullName, out workingFolder);
        IFolder folder = PakledTarget.Folder();
        arguments = $"--repository {folder.FullName} --target {target}";
    }
}
