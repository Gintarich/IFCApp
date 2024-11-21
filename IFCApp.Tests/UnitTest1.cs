using IFCApp.TeklaServices;

namespace IFCApp.Tests;

[TestClass]
public class TeklaConnectionTests
{
    [TestMethod]
    public void GetModelName_Should_ReturnName()
    {
        var sut = new TeklaProjectQuery(); 
        var result = sut.GetModelName();
        Assert.AreNotEqual(string.Empty, result );
    }
}