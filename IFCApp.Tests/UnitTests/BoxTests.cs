using IFCApp.Core;
using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using IFCApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

        [TestMethod]
        public void ShouldTransformToCS()
        {
            JsonModelSerializationService jSer = new("KUL-7AM-00-00-M3-BK-0001.json");
            Model model = jSer.Read();
            var idx = model.ElementMap[new Guid("3d02e2ba-4eae-4b60-bf7d-da5812921c52")];
            var wall = model.Elements[idx] as SandwichPanel;
            var openings = wall?.Openings;
            List<BBox> bboxes = new List<BBox>();
            foreach (var opening in openings)
            {
                var tformedBox = opening.Box.ToOtherCS(wall.Box.CS);
                bboxes.Add(tformedBox);
            }
            List<BBox> expectedBboxes = new()
            {
                new BBox([new Point3d(4640, 500, 2480),new Point3d(3590, -500, 780)]),
                new BBox([new Point3d(3040, 500, 2480),new Point3d(1990, -500, 780)]),
                new BBox([new Point3d(1330, 500, 2480),new Point3d(280, -500, 780)])
            };
            for (int i = 0; i < expectedBboxes.Count; i++)
            {
                Assert.AreEqual(bboxes[i].Min, expectedBboxes[i].Min);
                Assert.AreEqual(bboxes[i].Max, expectedBboxes[i].Max);
            }
        }
    }
}
