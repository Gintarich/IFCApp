using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.Tests.UnitTests;
[TestClass]
public class MatrixTests
{
    [TestMethod]
    public void TestReturnVectors()
    {
        var mat = new Matrix4d( new double[,]
        { 
            {1,0,0,0}, 
            {0,1,0,0}, 
            {0,0,1,0}, 
            {0,0,0,1}});
        Assert.AreEqual(mat.XAxis,Vector3d.XAxis);
        Assert.AreEqual(mat.YAxis,Vector3d.YAxis);
        Assert.AreEqual(mat.ZAxis,Vector3d.ZAxis);
    }
    [TestMethod]
    public void ShouldTransLate()
    {
        var sut = Matrix4d.Translation(100, 100, 0);
        var vec = new Point3d(0, 0, 0);
        var transformedPoint = sut.Apply(vec);
        var expected = new Point3d(100, 100, 0);
        Assert.AreEqual(expected, transformedPoint);
    }
    [TestMethod]
    public void ShouldRotate_AroundZ90()
    {
        var sut = Matrix4d.RotationZ(90);
        var pt = new Point3d(100, 100, 0);
        var transformedPoint = sut.Apply(pt);
        var expected = new Point3d(-100, 100, 0);
        Assert.AreEqual(expected, transformedPoint);
    }
    [TestMethod]
    public void ShouldRotate_AroundZ_minus90()
    {
        var sut = Matrix4d.RotationZ(-90);
        var pt = new Point3d(100, 100, 0);
        var transformedPoint = sut.Apply(pt);
        var expected = new Point3d(100, -100, 0);
        Assert.AreEqual(expected, transformedPoint);
    }

    [TestMethod]
    public void ShouldRotate_59Degrees()
    {
        var rot = Matrix4d.RotationZ(31);
        var trans = Matrix4d.Translation(375689000, 315329000, 32.20);
        var tot = trans.Combine(rot);
        var inverse = tot.Inverse();
    }

    [TestMethod]
    public void Should_Define_Coordinate_System()
    {
        // COORDINATION SYSTEM DEFINITION
        // | x1 y1 z1 T1 |
        // | x2 y2 z2 T2 |
        // | x3 y3 z3 T3 | 
        // | 0  0  0  1  |
        // In this example is CS rotated 30 degrees and moved 100 y dirrection
        var cs = new double[,]
        {
            { 0.866025,-0.5,      0, 0  },
            { 0.5,      0.866025, 0, 100},
            { 0,        0,        1, 0  },
            { 0,        0,        0, 1  }
        };
        var sut = new Matrix4d(cs);
        var pt = new Point3d(100, 100, 0);
        var transformedPt = sut.Apply(pt);
        transformedPt.Round(1);
        var expected = new Point3d(36.6, 236.6, 0);
        Assert.AreEqual(expected, transformedPt);
    }
    [TestMethod]
    public void Should_Combine()
    {
        var m1 = new double[,]
        {
            {1, 0, 0, 100},
            {0, 1, 0, 100},
            {0, 0, 1, 0  },
            {0, 0, 0, 1  }
        };
        var m2 = new double[,]
        {
            { 0.866025,-0.5,      0, 0 },
            { 0.5,      0.866025, 0, 0 },
            { 0,        0,        1, 0 },
            { 0,        0,        0, 1 }
        };
        var m3 = new double[,]
        {
            {1, 0, 0, 0  },
            {0, 1, 0, 100},
            {0, 0, 1, 0  },
            {0, 0, 0, 1  }
        };
        var mat1 = new Matrix4d(m1);
        var mat2 = new Matrix4d(m2);
        var mat3 = new Matrix4d(m3);
        //NEED to apply matrixes in reverse order (Last transformation first)
        var matrix = mat3.Combine(mat2).Combine(mat1);

        var transformed = matrix.Apply(new Point3d(0, 0, 0));
        transformed.Round(1);
        var expected = new Point3d(36.6, 236.6, 0);
        Assert.AreEqual(expected, transformed);
    }

    [TestMethod]
    public void ShouldInverseOne()
    {
        double[,] m1 =
        {
            { 4, 7, 2, 3 },
            { 3, 4, 6, 2 },
            { 2, 1, 5, 3 },
            { 1, 3, 4, 6 }
        };

        double[,] m2 =
        {
            {0.33, -0.48, 0.75, -0.38},
            {-0.03, 0.31, -0.48, 0.15},
            {-0.17, 0.29, -0.12, 0.05},
            {0.07, -0.27, 0.20, 0.12}
        };

        var matrix = new Matrix4d(m1);
        Matrix4d inverse = matrix.Inverse();
        Matrix4d expected = new Matrix4d(m2);
        inverse.Round(2);

        var ar1 = inverse.GetData();
        var ar2 = expected.GetData();

        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                Assert.AreEqual( ar1 [i,j], ar2 [i,j], 0.01);
            }
        }
    }
}
