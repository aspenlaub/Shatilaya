using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class CakeToolCheck {
    [TestMethod]
    public void CakeToolMatchesTargetFramework() {
        IContainer container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty("Shatilaya").Build();
        IDotNetCakeInstaller installer = new DotNetCakeInstaller(container.Resolve<Gitty.Interfaces.IProcessRunner>());
        var errorsAndInfos = new ErrorsAndInfos();
        bool doesGlobalCakeToolVersionMatchTargetFramework = installer.DoesGlobalCakeToolVersionMatchTargetFramework(true, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            return;
        }

        installer.UpdateGlobalDotNetCakeToMatchTargetFrameworkIfNecessary(errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);

        doesGlobalCakeToolVersionMatchTargetFramework = installer.DoesGlobalCakeToolVersionMatchTargetFramework(false, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            // ReSharper disable once RedundantJumpStatement
            return;
        }
    }
}
