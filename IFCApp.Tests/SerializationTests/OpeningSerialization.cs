using IFCApp.Core.Elements;
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
    public class OpeningSerialization
    {
        private const double Tolerance = 1e-9;

        [TestMethod]
        public void SerializeOpening_ShouldReturnCorrectJson()
        {
            // Arrange
            var startPoint = new Point3d(-300, -200, -500);
            var endPoint = new Point3d(300, 200, 500);
            var opening = new Opening(startPoint, endPoint, 123);

            // Act
            var json = JsonSerializer.Serialize(opening);

            // Assert
            var expectedJson = "{\"Box\":{\"Type\":\"BBox\",\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"FatherID\":123}";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeOpening_ShouldReturnCorrectOpening()
        {
            // Arrange
            var json = "{\"Box\":{\"Type\":\"BBox\",\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"FatherID\":123}";

            // Act
            var opening = JsonSerializer.Deserialize<Opening>(json);

            // Assert
            var expectedStartPoint = new Point3d(-300, -200, -500);
            var expectedEndPoint = new Point3d(300, 200, 500);
            var expectedBBox = new BBox(new List<Point3d> { expectedStartPoint, expectedEndPoint });
            var expectedOpening = new Opening(expectedBBox, 123);

            AssertHelpers.AssertOpeningAreEqual(expectedOpening, opening, Tolerance);
        }
    }
}
