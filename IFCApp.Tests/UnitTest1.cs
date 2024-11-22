using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Utils;
using Tekla.Structures.Geometry3d;

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
    [TestMethod]
    public void ShouldDrawCube()
    {
        TeklaGraphicsDrawerService drawer = new TeklaGraphicsDrawerService();
        drawer.DrawCube(new Point());
    }
}