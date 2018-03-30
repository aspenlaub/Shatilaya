using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetFeedListerTest {
        [TestMethod]
        public void CanFindPakledPackages() {
            const string feedUrl = "https://www.aspenlaub.net/nuget";
            const string packageId = "Aspenlaub.Net.GitHub.CSharp.Pakled";
            var sut = new NugetFeedLister();
            var packages = sut.ListReleasedPackages(feedUrl, packageId);
            Assert.IsTrue(packages.Count > 5);
            foreach (var package in packages) {
                Assert.AreEqual(packageId, package.Id);
            }
        }
    }
}
