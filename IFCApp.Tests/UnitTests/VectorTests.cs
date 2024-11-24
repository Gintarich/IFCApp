using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.Tests.UnitTests;
[TestClass]
public class VectorTests
{
    [TestMethod]
    public void EnsureStaticFields()
    {
        var axis = Vector3d.xAxis;
        var copy = Vector3d.xAxis;

        axis.X = 100;
        axis.Y = 100;
    }

    [TestMethod]
    public void EnsureCrossProduct()
    {
        var xAxis = Vector3d.xAxis;
        var zAxis = Vector3d.zAxis;

        var testVec = zAxis.Cross(xAxis);
        var expected = Vector3d.yAxis;

        Assert.AreEqual(expected, testVec);
    }
}
