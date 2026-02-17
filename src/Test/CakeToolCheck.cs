using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class CakeToolCheck {
    [TestMethod]
    public void CakeToolMatchesTargetFramework() {
        IContainer container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty("Shatilaya").Build();
        IDotNetCakeInstaller installer = container.Resolve<IDotNetCakeInstaller>();
        var errorsAndInfos = new ErrorsAndInfos();
        bool doesGlobalCakeToolVersionMatchTargetFramework = installer.DoesGlobalCakeToolVersionMatchTargetFramework(true, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            return;
        }

        installer.UpdateGlobalDotNetCakeToMatchTargetFrameworkIfNecessary(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());

        doesGlobalCakeToolVersionMatchTargetFramework = installer.DoesGlobalCakeToolVersionMatchTargetFramework(false, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            // ReSharper disable once RedundantJumpStatement
            return;
        }
    }
}
