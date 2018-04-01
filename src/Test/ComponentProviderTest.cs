﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ComponentProviderTest {

        [TestMethod]
        public void CanProvideComponents() {
            var sut = new ComponentProvider();
            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.CakeRunner);
            Assert.IsNotNull(sut.DependencyTreeBuilder);
            Assert.IsNotNull(sut.GitUtilities);
            Assert.IsNotNull(sut.LatestBuildCakeScriptProvider);
            Assert.IsNotNull(sut.NugetConfigReader);
            Assert.IsNotNull(sut.NugetFeedLister);
            Assert.IsNotNull(sut.NugetPackageInstaller);
            Assert.IsNotNull(sut.NugetPackageRestorer);
            Assert.IsNotNull(sut.NugetPackageToPushFinder);
            Assert.IsNotNull(sut.NuSpecCreator);
            Assert.IsNotNull(sut.ObsoletePackageFinder);
            Assert.IsNotNull(sut.PackageConfigsScanner);
            Assert.IsNotNull(sut.ProcessRunner);
            Assert.IsNotNull(sut.ProjectFactory);
        }
    }
}
