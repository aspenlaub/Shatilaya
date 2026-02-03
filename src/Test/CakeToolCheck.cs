using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Fusion.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class CakeToolCheck {
    [TestMethod]
    public void CakeToolMatchesTargetFramework() {
        IContainer container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty("Shatilaya", new DummyCsArgumentPrompter()).Build();
        IDotNetCakeInstaller installer = container.Resolve<IDotNetCakeInstaller>();
        var errorsAndInfos = new ErrorsAndInfos();
        bool doesGlobalCakeToolVersionMatchTargetFramework = installer.DoesGlobalCakeToolVersionMatchTargetFramework(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            return;
        }

        installer.UpdateGlobalDotNetCakeToMatchTargetFrameworkIfNecessary(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());

        doesGlobalCakeToolVersionMatchTargetFramework = installer.DoesGlobalCakeToolVersionMatchTargetFramework(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            return;
        }
    }
}
