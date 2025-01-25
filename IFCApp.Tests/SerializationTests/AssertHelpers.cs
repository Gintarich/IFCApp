using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace IFCApp.Tests.SerializationTests
{
    public static class AssertHelpers
    {
        public static void AssertOpeningAreEqual(Opening expected, Opening actual, double tolerance)
        {
            AssertBBoxAreEqual(expected.Box, actual.Box, tolerance);
            Assert.AreEqual(expected.FatherID, actual.FatherID);
        }

        public static void AssertBBoxAreEqual(BBox expected, BBox actual, double tolerance)
        {
            AssertPointAreEqual(expected.Min, actual.Min, tolerance);
            AssertPointAreEqual(expected.Max, actual.Max, tolerance);
            AssertMatrixAreEqual(expected.CS.GetData(), actual.CS.GetData(), tolerance);
        }

        public static void AssertPointAreEqual(Point3d expected, Point3d actual, double tolerance)
        {
            Assert.IsTrue(AreDoublesEqual(expected.X, actual.X, tolerance), "Mismatch in X coordinate");
            Assert.IsTrue(AreDoublesEqual(expected.Y, actual.Y, tolerance), "Mismatch in Y coordinate");
            Assert.IsTrue(AreDoublesEqual(expected.Z, actual.Z, tolerance), "Mismatch in Z coordinate");
        }

        public static void AssertMatrixAreEqual(double[,] expected, double[,] actual, double tolerance)
        {
            Assert.AreEqual(expected.GetLength(0), actual.GetLength(0));
            Assert.AreEqual(expected.GetLength(1), actual.GetLength(1));

            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    Assert.IsTrue(AreDoublesEqual(expected[i, j], actual[i, j], tolerance), $"Mismatch at element [{i},{j}]");
                }
            }
        }

        public static void AssertJsonAreEqual(string expectedJson, string actualJson, double tolerance)
        {
            var expectedPoint = JsonSerializer.Deserialize<Point3d>(expectedJson);
            var actualPoint = JsonSerializer.Deserialize<Point3d>(actualJson);

            AssertPointAreEqual(expectedPoint, actualPoint, tolerance);
        }

        private static bool AreDoublesEqual(double a, double b, double tolerance)
        {
            return Math.Abs(a - b) <= tolerance;
        }
    }
}
