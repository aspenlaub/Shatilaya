using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using IProcessRunner = Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces.IProcessRunner;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Test;

[TestClass]
public class ShatilayaCakeFrostingCommandLineTest : ShatilayaCakeFrostingTestBase {
    [TestMethod]
    public void CanCleanRestorePull() {
        IFolder? folder = PakledTarget.Folder();
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        Assert.IsFalse(string.IsNullOrEmpty(assemblyLocation));
        var workingFolder = new Folder(assemblyLocation.Substring(0, assemblyLocation.LastIndexOf('\\')));
        string[] executableFullNames = Directory.GetFiles(workingFolder.FullName, "*Shatilaya*.exe");
        Assert.HasCount(1, executableFullNames);
        string executableFullName = executableFullNames[0];
        string arguments = $"--repository {folder.FullName} --target CleanRestorePull";
        IProcessRunner processRunner = Container.Resolve<IProcessRunner>();
        var errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess(executableFullName, arguments, workingFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        List<string> expectedInfos = [
          "Cleaning", "Restoring", "Pulling"
        ];
        foreach (string expectedInfo in expectedInfos) {
            Assert.IsTrue(errorsAndInfos.Infos.Any(x => x.StartsWith(expectedInfo)), $"No info starts with {expectedInfo}");
        }
    }
}
