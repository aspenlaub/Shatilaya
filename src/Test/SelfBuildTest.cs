using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class SelfBuildTest {
        protected static TestTargetFolder ShatilayaTarget = new TestTargetFolder(nameof(SelfBuildTest), "Shatilaya");

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ShatilayaTarget.DeleteCakeFolder();
            ShatilayaTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            ShatilayaTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            ShatilayaTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            ShatilayaTarget.Delete();
        }

        [TestMethod, Ignore]
        public void CanBuildMyself() {
            const string url = "https://github.com/aspenlaub/Shatilaya.git";
            Repository.Clone(url, ShatilayaTarget.FullName(), new CloneOptions { BranchName = "master" });
            IList<string> cakeMessages, cakeErrors;
            ShatilayaTarget.RunBuildCakeScript(out cakeMessages, out cakeErrors);
            Assert.IsFalse(cakeErrors.Any(), string.Join("\r\n", cakeErrors));
        }
    }
}