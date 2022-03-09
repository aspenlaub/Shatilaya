﻿using System.Linq;
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

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NuclideVersionCheck {
        private static IContainer Container;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            Container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty(new DummyCsArgumentPrompter()).Build();
        }

        [TestMethod]
        public void BuildCakeUsesRightPackageVersion() {
            var version = typeof(INugetPackageUpdater).Assembly.GetName().Version;
            Assert.IsNotNull(version);
            Assert.IsTrue(version.ToString().StartsWith("2.0."));
            var errorsAndInfos = new ErrorsAndInfos();
            var buildCake = Container.Resolve<IEmbeddedCakeScriptReader>().ReadCakeScriptFromAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard, errorsAndInfos).Split("\n");
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsTrue(buildCake.Any(s => s.Contains("Fusion") & s.Contains($"version={version}")),
                $"Build cake does not use Fusion version {version}");
        }
    }
}
