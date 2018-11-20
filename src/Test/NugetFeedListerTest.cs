using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetFeedListerTest {
        [TestMethod, Ignore]
        public async Task CanFindPakledPackages() {
            const string feedUrl = "https://www.aspenlaub.net/nuget";
            const string packageId = "Aspenlaub.Net.GitHub.CSharp.Pakled";
            INugetFeedLister sut = new NugetFeedLister();
            var packages = (await sut.ListReleasedPackagesAsync(feedUrl, packageId)).ToList();
            Assert.IsTrue(packages.Count > 5);
            foreach (var package in packages) {
                Assert.AreEqual(packageId, package.Identity.Id);
            }
        }
    }
}
