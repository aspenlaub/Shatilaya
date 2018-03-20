using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ComponentProviderTest {

        [TestMethod]
        public void CanProvideComponents() {
            var sut = new ComponentProvider();
            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.CakeRunner);
            Assert.IsNotNull(sut.DependencyTreeBuilder);
            Assert.IsNotNull(sut.FolderDeleter);
            Assert.IsNotNull(sut.FolderUpdater);
            Assert.IsNotNull(sut.GitUtilities);
            Assert.IsNotNull(sut.LatestBuildCakeScriptProvider);
            Assert.IsNotNull(sut.NugetPackageRestorer);
            Assert.IsNotNull(sut.NuSpecCreator);
            Assert.IsNotNull(sut.ObsoletePackageFinder);
            Assert.IsNotNull(sut.PackageConfigsScanner);
        }
    }
}
