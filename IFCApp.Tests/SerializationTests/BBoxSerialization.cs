using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IFCApp.Tests.SerializationTests
{
    [TestClass]
    public class BBoxSerialization
    {
        private const double Tolerance = 1e-9;

        [TestMethod]
        public void SerializeBBox_ShouldReturnCorrectJson()
        {
            // Arrange
            var points = new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };
            var cs = new Matrix4d(new double[,]
            {
                {1, 0, 0, 12000 },
                {0, 1, 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });
            var bbox = new BBox(points, cs);

            // Act
            var json = JsonSerializer.Serialize(bbox);

            // Assert
            var expectedJson = "{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,12000],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeBBox_ShouldReturnCorrectBBox()
        {
            // Arrange
            var json = "{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,12000],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";

            // Act
            var bbox = JsonSerializer.Deserialize<BBox>(json);

            // Assert
            var expectedPoints = new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(50, 50, 50),
                new Point3d(300, 200, 500)
            };
            var expectedCS = new Matrix4d(new double[,]
            {
                {1, 0, 0, 12000 },
                {0, 1, 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });
            var expectedBBox = new BBox(expectedPoints, expectedCS);

            AssertHelpers.AssertBBoxAreEqual(expectedBBox, bbox, Tolerance);
        }
    }
}
