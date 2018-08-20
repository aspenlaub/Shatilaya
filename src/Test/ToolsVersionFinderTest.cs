using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ToolsVersionFinderTest {
        protected const string Vs7RegistryKey = @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7";

        [TestMethod]
        public void CanGetLatestAvailableToolsVersion() {
            var componentProviderMock = new Mock<IComponentProvider>();
            var executableFinderMock = new Mock<IExecutableFinder>();
            componentProviderMock.Setup(c => c.ExecutableFinder).Returns(executableFinderMock.Object);
            executableFinderMock.Setup(e => e.HaveVs7()).Returns(false);
            IToolsVersionFinder sut = new ToolsVersionFinder(componentProviderMock.Object);
            Assert.AreEqual(14, sut.LatestAvailableToolsVersion());

            executableFinderMock.Setup(e => e.HaveVs7()).Returns(true);
            executableFinderMock.Setup(e => e.FindMsTestExe(It.IsAny<int>())).Returns<int>(v => v <= 17 ? "." : "");
            executableFinderMock.Setup(e => e.FindVsTestExe(It.IsAny<int>())).Returns<int>(v => v <= 17 ? "." : "");
            Assert.AreEqual(17, sut.LatestAvailableToolsVersion());
        }
    }
}
