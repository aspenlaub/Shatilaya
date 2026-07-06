using System.Threading;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class DotNetCakeInstallerTest {
    protected IDotNetCakeInstaller Sut;

    [TestInitialize]
    public void Initialize() {
        IContainer container = new ContainerBuilder().UseFusionNuclideProtchAndGitty("Gitty").Build();
        Sut = new DotNetCakeInstaller(container.Resolve<Gitty.Interfaces.IProcessRunner>());
    }

    [TestMethod]
    public async Task CanInstallGlobalDotNetCakeIfNecessary() {
        var errorsAndInfos = new ErrorsAndInfos();
        Inconclusive inconclusive = new();
        await Sut.InstallOrUpdateGlobalDotNetCakeIfNecessaryAsync(errorsAndInfos, inconclusive, CancellationToken.None);
        if (inconclusive.IsInconclusive) {
            Assert.Inconclusive();
        } else {
            Assert.That.ThereWereNoErrors(errorsAndInfos);
        }
    }

    [TestMethod]
    public async Task ProvenGlobalDotNetCakeIsInstalled() {
        var errorsAndInfos = new ErrorsAndInfos();
        bool isInstalled = await Sut.IsProvenGlobalDotNetCakeInstalledAsync(errorsAndInfos, CancellationToken.None);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.IsTrue(isInstalled);
    }

    [TestMethod]
    public async Task CanCheckIfGlobalCakeToolVersionMatchesTargetFramework() {
        var errorsAndInfos = new ErrorsAndInfos();
        bool matches = await Sut.DoesGlobalCakeToolVersionMatchTargetFrameworkAsync(false, errorsAndInfos, CancellationToken.None);
        if (matches) {
            Assert.That.ThereWereNoErrors(errorsAndInfos);
        }
        Assert.AreEqual(DotNetCakeInstaller.CakeToolVersionMatchingCompiledTargetFramework ==
            DotNetCakeInstaller.ProvenCakeToolVersion, matches);
    }
}