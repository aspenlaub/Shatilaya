using System;
using System.Linq;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Fusion.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;

[assembly: DoNotParallelize]
namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class FusionVersionCheck {
    private static IContainer _container;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) {
        _container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty("Shatilaya", new DummyCsArgumentPrompter()).Build();
    }

    [TestMethod]
    public void BuildCakeUsesRightPackageVersion() {
        Version version = typeof(INugetPackageUpdater).Assembly.GetName().Version;
        Assert.IsNotNull(version);
        Assert.StartsWith("2.0.", version.ToString());
        var errorsAndInfos = new ErrorsAndInfos();
        string[] buildCake = _container.Resolve<IEmbeddedCakeScriptReader>().ReadCakeScriptFromAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard, errorsAndInfos).Split("\n");
        Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        const string packageId = "Fusion-DotnetNine";
        Assert.IsTrue(buildCake.Any(s => s.Contains(packageId) & s.Contains($"version={version}")),
            $"Build cake does not use {packageId} version {version}");
    }
}