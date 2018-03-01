using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class CakeRunnerTest {
        protected ICakeRunner Sut;

        [TestInitialize]
        public void Initialize() {
            Sut = new CakeRunner();
        }

        [TestMethod]
        public void CanCallScriptWithoutErrors() {
        }

        [TestMethod]
        public void CanCallScriptWithErrors() {
        }
    }
}
