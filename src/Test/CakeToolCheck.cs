using System.Threading;
using System.Threading.Tasks;
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
    public async Task CakeToolMatchesTargetFramework() {
        IContainer container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty("Shatilaya").Build();
        IDotNetCakeInstaller installer = new DotNetCakeInstaller(container.Resolve<Gitty.Interfaces.IProcessRunner>());
        var errorsAndInfos = new ErrorsAndInfos();
        bool doesGlobalCakeToolVersionMatchTargetFramework =
            await installer.DoesGlobalCakeToolVersionMatchTargetFrameworkAsync(true, errorsAndInfos, CancellationToken.None);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            return;
        }

        await installer.UpdateGlobalDotNetCakeToMatchTargetFrameworkIfNecessaryAsync(errorsAndInfos, CancellationToken.None);
        Assert.That.ThereWereNoErrors(errorsAndInfos);

        doesGlobalCakeToolVersionMatchTargetFramework =
            await installer.DoesGlobalCakeToolVersionMatchTargetFrameworkAsync(false, errorsAndInfos, CancellationToken.None);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        if (doesGlobalCakeToolVersionMatchTargetFramework) {
            // ReSharper disable once RedundantJumpStatement
            return;
        }
    }
}
