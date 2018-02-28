using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ComponentProviderTest {

        [TestMethod]
        public void CanProvideComponents() {
            var sut = new ComponentProvider();
            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.FolderDeleter);
        }
    }
}
