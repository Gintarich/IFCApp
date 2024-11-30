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
                new Point3d(-300,-200,-500),
                new Point3d(50,50,50),
                new Point3d(300,200,500)
            };
            Matrix4d cs = new Matrix4d(new double[,] {
                    {1,0,0,12000 },
                    {0,1,0,0 },
                    {0,0,1,0 },
                    {0,0,0,1 } });

            var box = new BBox(points, cs);
            var min = box.Min;
            var max = box.Max;
            Assert.AreEqual(box.Min.X, 11700, 0.001);
            Assert.AreEqual(box.Max.X, 12300, 0.001);
        }
    }
}
