using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.Tests.UnitTests
{
    [TestClass]
    public class BoxTests
    {
        [TestMethod]
        public void ShouldCreateBox()
        {
            List<Point3d> points = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };
            Matrix4d cs = new Matrix4d(new double[,] {
                {1, 0, 0, 12000 },
                {0, 1, 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });

            var box = new BBox(points, cs);
            var min = box.GetMin();
            var max = box.GetMax();
            Assert.AreEqual(11700, min.X, 0.001);
            Assert.AreEqual(12300, max.X, 0.001);
        }

        [TestMethod]
        public void ShouldTrimBoxWidth()
        {
            List<Point3d> points = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            var box = new BBox(points);
            box.TrimBoxWidth(100);

            Assert.AreEqual(-50, box.Min.Y, 0.001);
            Assert.AreEqual(50, box.Max.Y, 0.001);
        }

        [TestMethod]
        public void ShouldTrimBoxLength()
        {
            List<Point3d> points = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            var box = new BBox(points);
            box.TrimBoxLength(100);

            Assert.AreEqual(-50, box.Min.X, 0.001);
            Assert.AreEqual(50, box.Max.X, 0.001);
        }

        [TestMethod]
        public void ShouldTrimBoxHeight()
        {
            List<Point3d> points = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            var box = new BBox(points);
            box.TrimBoxHeight(100);

            Assert.AreEqual(-50, box.Min.Z, 0.001);
            Assert.AreEqual(50, box.Max.Z, 0.001);
        }

        [TestMethod]
        public void ShouldSetHeight()
        {
            List<Point3d> points = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            var box = new BBox(points);
            box.SetHeight(100);

            Assert.AreEqual(100, box.Max.Z, 0.001);
        }

        [TestMethod]
        public void ShouldIntersect()
        {
            List<Point3d> points1 = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            List<Point3d> points2 = new List<Point3d>()
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            };

            var box1 = new BBox(points1);
            var box2 = new BBox(points2);

            Assert.IsTrue(box1.Intersects(box2));
        }

        [TestMethod]
        public void ShouldNotIntersect()
        {
            List<Point3d> points1 = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            List<Point3d> points2 = new List<Point3d>()
            {
                new Point3d(1000, 1000, 1000),
                new Point3d(2000, 2000, 2000)
            };

            var box1 = new BBox(points1);
            var box2 = new BBox(points2);

            Assert.IsFalse(box1.Intersects(box2));
        }

        [TestMethod]
        public void ShouldCalculateOverlapVolume()
        {
            List<Point3d> points1 = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            List<Point3d> points2 = new List<Point3d>()
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            };

            var box1 = new BBox(points1);
            var box2 = new BBox(points2);

            double expectedVolume = 200 * 200 * 200; // Overlapping volume
            Assert.AreEqual(expectedVolume, box1.OverlapVolume(box2), 0.001);
        }

        [TestMethod]
        public void ShouldCheckIfParallel()
        {
            List<Point3d> points1 = new List<Point3d>()
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };

            List<Point3d> points2 = new List<Point3d>()
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            };

            var box1 = new BBox(points1);
            var box2 = new BBox(points2);

            Assert.IsTrue(box1.IsParallel(box2));
        }
    }
}
