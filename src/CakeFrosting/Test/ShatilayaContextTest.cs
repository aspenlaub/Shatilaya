using Cake.Core;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Test;

[TestClass]
public sealed class ShatilayaContextTest {
    [TestMethod]
    public void CanCreateShatilayaContext() {
        var contextMock = new Mock<ICakeContext>();
        ICakeContext context = contextMock.Object;
        var shatilayaContext = new ShatilayaContext(context);
        Assert.IsNotNull(shatilayaContext);
    }
}
