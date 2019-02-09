using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty.TestUtilities;
using Aspenlaub.Net.GitHub.CSharp.Nuclide;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Interfaces;
using IContainer = Autofac.IContainer;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NuclideVersionCheck {
        private static IContainer vContainer;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            vContainer = new ContainerBuilder().UseGittyTestUtilities().UseNuclideProtchAndGitty().Build();
        }

        [TestMethod]
        public void BuildCakeUsesRightPackageVersion() {
            var version = typeof(INugetPackageToPushFinder).Assembly.GetName().Version;
            Assert.IsTrue(version.ToString().StartsWith("1.0."));
            var buildCake = vContainer.Resolve<IEmbeddedCakeScriptReader>().ReadCakeScriptFromAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard).Split("\n");
            Assert.IsTrue(buildCake.Any(s => s.Contains("Nuclide") & s.Contains($"version={version.ToString()}")),
                $"Build cake does not use Nuclide version {version}");
        }
    }
}
