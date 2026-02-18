using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty.TestUtilities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Autofac;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

public class ShatilayaTestBase {
    protected TestTargetFolder PakledTarget = new(nameof(ShatilayaContextTest), "");
    protected IContainer Container = new ContainerBuilder().Build();

    [TestInitialize]
    public void Initialize() {
        PakledTarget = new TestTargetFolder(nameof(ShatilayaContextTest), "Pakled");
        Container = new ContainerBuilder()
            .UseGittyTestUtilities()
            .UseFusionNuclideProtchAndGitty("Shatilaya").Build();
        Cleanup();
        const string url = "https://github.com/aspenlaub/Pakled.git";
        var errorsAndInfos = new ErrorsAndInfos();
        Container.Resolve<IGitUtilities>().Clone(url, "master", PakledTarget.Folder(),
            new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsPlusRelevantInfos());
        Container.Resolve<IGitUtilities>().Pull(PakledTarget.Folder(), "Shatilaya tester", "shatilayatester@aspenlaub.net");
    }

    [TestCleanup]
    public void Cleanup() {
        if (PakledTarget.Exists()) {
            PakledTarget.Delete();
        }
    }
}
