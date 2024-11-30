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
        var axis = Vector3d.XAxis;
        var copy = Vector3d.XAxis;

        axis.X = 100;
        axis.Y = 100;
    }

    [TestMethod]
    public void EnsureCrossProduct()
    {
        var xAxis = Vector3d.XAxis;
        var zAxis = Vector3d.ZAxis;

        var testVec = zAxis.Cross(xAxis);
        var expected = Vector3d.YAxis;

        Assert.AreEqual(expected, testVec);
    }
}
