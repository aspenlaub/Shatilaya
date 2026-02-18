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
    public void CanInstallGlobalDotNetCakeIfNecessary() {
        var errorsAndInfos = new ErrorsAndInfos();
        Sut.InstallOrUpdateGlobalDotNetCakeIfNecessary(errorsAndInfos, out bool inconclusive);
        if (inconclusive) {
            Assert.Inconclusive();
        } else {
            Assert.That.ThereWereNoErrors(errorsAndInfos);
        }
    }

    [TestMethod]
    public void ProvenGlobalDotNetCakeIsInstalled() {
        var errorsAndInfos = new ErrorsAndInfos();
        bool isInstalled = Sut.IsProvenGlobalDotNetCakeInstalled(errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.IsTrue(isInstalled);
    }

    [TestMethod]
    public void CanCheckIfGlobalCakeToolVersionMatchesTargetFramework() {
        var errorsAndInfos = new ErrorsAndInfos();
        bool matches = Sut.DoesGlobalCakeToolVersionMatchTargetFramework(false, errorsAndInfos);
        if (matches) {
            Assert.That.ThereWereNoErrors(errorsAndInfos);
        }
        Assert.AreEqual(DotNetCakeInstaller.CakeToolVersionMatchingCompiledTargetFramework ==
            DotNetCakeInstaller.ProvenCakeToolVersion, matches);
    }
}