using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ExecutableFinderTest {
        [TestMethod]
        public void CanFindExecutable() {
            var sut = new ExecutableFinder();
            var msTestFullPath = sut.FindMsTestExe(14);
            var vsTestFullPath = sut.FindVsTestExe(14);
            Assert.IsTrue(msTestFullPath != "" || vsTestFullPath != "");
        }
    }
}
