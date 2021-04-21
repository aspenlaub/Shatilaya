using System.Linq;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.FusionNet;
using Aspenlaub.Net.GitHub.CSharp.FusionNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.GittyNet;
using Aspenlaub.Net.GitHub.CSharp.GittyNet.Extensions;
using Aspenlaub.Net.GitHub.CSharp.GittyNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NuclideVersionCheck {
        private static IContainer vContainer;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            vContainer = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty(new DummyCsArgumentPrompter()).Build();
        }

        [TestMethod]
        public void BuildCakeUsesRightPackageVersion() {
            var version = typeof(INugetPackageUpdater).Assembly.GetName().Version;
            Assert.IsNotNull(version);
            Assert.IsTrue(version.ToString().StartsWith("2.0."));
            var errorsAndInfos = new ErrorsAndInfos();
            var buildCake = vContainer.Resolve<IEmbeddedCakeScriptReader>().ReadCakeScriptFromAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard, errorsAndInfos).Split("\n");
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsTrue(buildCake.Any(s => s.Contains("Fusion") & s.Contains($"version={version}")),
                $"Build cake does not use Fusion version {version}");
        }
    }
}
