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
    public class PointSerialization
    {
        private const double Tolerance = 1e-9;
        private const int DecimalPlaces = 9;

        [TestMethod]
        public void SerializePoint3d_ShouldReturnCorrectJson()
        {
            // Arrange
            var point = new Point3d(1.0, 2.0, 3.0);

            // Act
            var json = JsonSerializer.Serialize(point);

            // Assert
            var expectedJson = "{\"X\":1.0,\"Y\":2.0,\"Z\":3.0}";
            AssertHelpers.AssertJsonAreEqual(expectedJson, json, Tolerance);
        }

        [TestMethod]
        public void DeserializePoint3d_ShouldReturnCorrectPoint()
        {
            // Arrange
            var json = "{\"X\":1.0,\"Y\":2.0,\"Z\":3.0}";

            // Act
            var point = JsonSerializer.Deserialize<Point3d>(json);

            // Assert
            var expectedPoint = new Point3d(1.0, 2.0, 3.0);

            AssertHelpers.AssertPointAreEqual(expectedPoint, point, Tolerance);
        }

        [TestMethod]
        public void SerializePoint3d_WithDecimals_ShouldReturnCorrectJson()
        {
            // Arrange
            var point = new Point3d(1.123456789, 2.987654321, 3.456789123);

            // Act
            var json = JsonSerializer.Serialize(point);

            // Assert
            var expectedJson = "{\"X\":1.123456789,\"Y\":2.987654321,\"Z\":3.456789123}";
            AssertHelpers.AssertJsonAreEqual(expectedJson, json, Tolerance);
        }

        [TestMethod]
        public void DeserializePoint3d_WithDecimals_ShouldReturnCorrectPoint()
        {
            // Arrange
            var json = "{\"X\":1.123456789,\"Y\":2.987654321,\"Z\":3.456789123}";

            // Act
            var point = JsonSerializer.Deserialize<Point3d>(json);

            // Assert
            var expectedPoint = new Point3d(1.123456789, 2.987654321, 3.456789123);

            AssertHelpers.AssertPointAreEqual(expectedPoint, point, Tolerance);
        }
    }
}
