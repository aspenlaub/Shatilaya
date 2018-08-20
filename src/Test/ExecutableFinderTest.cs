using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ExecutableFinderTest {
        [TestMethod]
        public void CanFindExecutable() {
            IExecutableFinder sut = new ExecutableFinder();
            var msTestFullPath = sut.FindMsTestExe(14);
            var vsTestFullPath = sut.FindVsTestExe(14);
            Assert.IsTrue(msTestFullPath != "" || vsTestFullPath != "");
            if (!sut.HaveVs7()) { return; }

            msTestFullPath = sut.FindMsTestExe(15);
            vsTestFullPath = sut.FindVsTestExe(15);
            Assert.IsTrue(msTestFullPath != "" || vsTestFullPath != "");
        }
    }
}
