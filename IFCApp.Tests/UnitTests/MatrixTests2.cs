using System;
using IFCApp.Core.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IFCApp.Tests.UnitTests
{
    [TestClass]
    public class MatrixTests2
    {
        private void AssertMatricesAreEqual(double[,] expected, double[,] actual, double tolerance = 1e-9)
        {
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    Assert.IsTrue(Math.Abs(expected[i, j] - actual[i, j]) < tolerance,
                        $"Expected: {expected[i, j]}, Actual: {actual[i, j]} at position [{i},{j}]");
                }
            }
        }

        [TestMethod]
        public void TestDefaultConstructor()
        {
            var matrix = new Matrix4d();
            var expected = new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, matrix.GetData());
        }

        [TestMethod]
        public void TestCustomMatrixConstructor()
        {
            var customMatrix = new double[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            };
            var matrix = new Matrix4d(customMatrix);
            AssertMatricesAreEqual(customMatrix, matrix.GetData());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidCustomMatrixConstructor()
        {
            var invalidMatrix = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };
            var matrix = new Matrix4d(invalidMatrix);
        }

        [TestMethod]
        public void TestRotationX()
        {
            var angle = 90.0;
            var matrix = Matrix4d.RotationX(angle * Math.PI / 180.0);
            var expected = new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, matrix.GetData());
        }

        [TestMethod]
        public void TestRotationY()
        {
            var angle = 90.0;
            var matrix = Matrix4d.RotationY(angle * Math.PI / 180.0);
            var expected = new double[,]
            {
                { 0, 0, 1, 0 },
                { 0, 1, 0, 0 },
                { -1, 0, 0, 0 },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, matrix.GetData());
        }

        [TestMethod]
        public void TestRotationZ()
        {
            var angle = 90.0;
            var matrix = Matrix4d.RotationZ(angle);
            var expected = new double[,]
            {
                { 0, -1, 0, 0 },
                { 1, 0, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, matrix.GetData());
        }

        [TestMethod]
        public void TestTranslation()
        {
            var tx = 1.0;
            var ty = 2.0;
            var tz = 3.0;
            var matrix = Matrix4d.Translation(tx, ty, tz);
            var expected = new double[,]
            {
                { 1, 0, 0, tx },
                { 0, 1, 0, ty },
                { 0, 0, 1, tz },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, matrix.GetData());
        }

        [TestMethod]
        public void TestApplyVector()
        {
            var matrix = new Matrix4d();
            var vector = new double[] { 1, 2, 3 };
            var result = matrix.Apply(vector);
            CollectionAssert.AreEqual(vector, result);
        }

        [TestMethod]
        public void TestApplyPoint()
        {
            var matrix = new Matrix4d();
            var point = new Point3d(1, 2, 3);
            var result = matrix.Apply(point);
            Assert.AreEqual(point, result);
        }

        [TestMethod]
        public void TestApplyVector3d()
        {
            var matrix = new Matrix4d();
            var vector = new Vector3d(1, 2, 3);
            var result = matrix.Apply(vector);
            Assert.AreEqual(vector, result);
        }

        [TestMethod]
        public void TestCombine()
        {
            var matrix1 = new Matrix4d();
            var matrix2 = new Matrix4d();
            var result = matrix1.Combine(matrix2);
            var expected = new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, result.GetData());
        }

        [TestMethod]
        public void TestInverse()
        {
            var matrix = new Matrix4d();
            var result = matrix.Inverse();
            var expected = new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            AssertMatricesAreEqual(expected, result.GetData());
        }

        [TestMethod]
        public void TestRound()
        {
            var customMatrix = new double[,]
            {
                { 1.12345, 2.12345, 3.12345, 4.12345 },
                { 5.12345, 6.12345, 7.12345, 8.12345 },
                { 9.12345, 10.12345, 11.12345, 12.12345 },
                { 13.12345, 14.12345, 15.12345, 16.12345 }
            };
            var matrix = new Matrix4d(customMatrix);
            matrix.Round(2);
            var expected = new double[,]
            {
                { 1.12, 2.12, 3.12, 4.12 },
                { 5.12, 6.12, 7.12, 8.12 },
                { 9.12, 10.12, 11.12, 12.12 },
                { 13.12, 14.12, 15.12, 16.12 }
            };
            AssertMatricesAreEqual(expected, matrix.GetData());
        }
    }
}
