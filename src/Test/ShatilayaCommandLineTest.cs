using System.Collections.Generic;
using System.IO;
using System.Linq;
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
#pragma warning disable MSTEST0037
            Assert.IsTrue(errorsAndInfos.Infos.Any(x => x.Contains(expectedInfo)), $"No info starts with {expectedInfo}");
#pragma warning restore MSTEST0037
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
#pragma warning disable MSTEST0037
            Assert.IsTrue(errorsAndInfos.Infos.Any(x => x.Contains(expectedInfo)), $"No info starts with {expectedInfo}");
#pragma warning restore MSTEST0037
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
            "Passed: 3",
            "Copying Debug artifacts to master Debug binaries folder",
            "Building solution in Release",
            "Running unit and integration tests on Release artifacts",
            "Copying Release artifacts to master Release binaries folder"
        ];
        foreach (string expectedInfo in expectedInfos) {
#pragma warning disable MSTEST0037
            Assert.IsTrue(errorsAndInfos.Infos.Any(x => x.Contains(expectedInfo)), $"No info starts with {expectedInfo}");
#pragma warning restore MSTEST0037
        }
    }

    [TestMethod]
    public void CanBuildToTemp() {
        PutTogetherRunnerArguments("LittleThings", out string executableFullName, out string arguments, out Folder workingFolder);
        IProcessRunner processRunner = Container.Resolve<IProcessRunner>();
        var errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());

        for (int i = 0; i < 2; i ++) {
            PutTogetherRunnerArguments("DebugBuildToTemp", out executableFullName, out arguments, out workingFolder);
            errorsAndInfos = new ErrorsAndInfos();
            processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
            VerifyOutputToTemporaryFolder("Debug", errorsAndInfos);

            PutTogetherRunnerArguments("ReleaseBuildToTemp", out executableFullName, out arguments, out workingFolder);
            errorsAndInfos = new ErrorsAndInfos();
            processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
            VerifyOutputToTemporaryFolder("Release", errorsAndInfos);
        }

        Assert.Contains("Output folder exists, cleaning up", errorsAndInfos.Infos);
    }

    private void PutTogetherRunnerArguments(string target, out string executableFullName, out string arguments, out Folder workingFolder) {
        ShatilayaFinder.FindShatilaya(out executableFullName, out workingFolder);
        IFolder folder = PakledTarget.Folder();
        arguments = $"--repository {folder.FullName} --target {target} --verbosity verbose";
    }

    private static void VerifyOutputToTemporaryFolder(string debugOrRelease, IErrorsAndInfos errorsAndInfos) {
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        const string outputFolderTag = "Output folder is: ";
        string line = errorsAndInfos.Infos.SingleOrDefault(s => s.StartsWith(outputFolderTag));
        Assert.IsFalse(string.IsNullOrEmpty(line), "Output folder could not be found");
        var outputFolder = new Folder(line.Substring(outputFolderTag.Length));
        Assert.IsTrue(outputFolder.Exists(), $"Output folder {outputFolder.FullName} does not exist");
        string expectedSuffix = @$"\src\temp\bin\{debugOrRelease}";
        Assert.EndsWith(expectedSuffix, outputFolder.FullName, $"Output folder {outputFolder.FullName} does not end with {expectedSuffix}");
        var outputFiles = Directory.GetFiles(outputFolder.FullName, "Aspenlaub.Net.GitHub.CSharp.Pakled.*", SearchOption.AllDirectories).ToList();
        Assert.HasCount(7, outputFiles);
        Assert.HasCount(2, outputFiles.Where(x => x.EndsWith(".dll")));
        Assert.HasCount(2, outputFiles.Where(x => x.EndsWith(".pdb")));
        Assert.HasCount(3, outputFiles.Where(x => x.EndsWith(".json")));
    }

}
