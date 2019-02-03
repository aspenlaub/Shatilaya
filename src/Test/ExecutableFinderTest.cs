using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ExecutableFinderTest {
        [TestMethod]
        public void CanFindExecutable() {
            IExecutableFinder sut = new ExecutableFinder();
            Assert.IsTrue(sut.HaveVs7());

            var msTestFullPath = sut.FindMsTestExe(15);
            var vsTestFullPath = sut.FindVsTestExe(15);
            Assert.IsTrue(msTestFullPath != "" || vsTestFullPath != "");
        }
    }
}
