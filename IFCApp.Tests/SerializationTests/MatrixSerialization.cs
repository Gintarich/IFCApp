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
    public class MatrixSerialization
    {
        private const double Tolerance = 1e-9;

        [TestMethod]
        public void SerializeMatrix4d_ShouldReturnCorrectJson()
        {
            // Arrange
            var matrix = new Matrix4d(new double[,]
            {
                { 1.0, 2.0, 3.0, 4.0 },
                { 5.0, 6.0, 7.0, 8.0 },
                { 9.0, 10.0, 11.0, 12.0 },
                { 13.0, 14.0, 15.0, 16.0 }
            });

            // Act
            var json = JsonSerializer.Serialize(matrix);

            // Assert
            var expectedJson = "{\"Matrix\":[[1,2,3,4],[5,6,7,8],[9,10,11,12],[13,14,15,16]]}";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeMatrix4d_ShouldReturnCorrectMatrix()
        {
            // Arrange
            var json = "{\"Matrix\":[[1,2,3,4],[5,6,7,8],[9,10,11,12],[13,14,15,16]]}";

            // Act
            var matrix = JsonSerializer.Deserialize<Matrix4d>(json);

            // Assert
            var expectedMatrix = new double[,]
            {
                { 1.0, 2.0, 3.0, 4.0 },
                { 5.0, 6.0, 7.0, 8.0 },
                { 9.0, 10.0, 11.0, 12.0 },
                { 13.0, 14.0, 15.0, 16.0 }
            };

            AssertHelpers.AssertMatrixAreEqual(expectedMatrix, matrix.GetData(), Tolerance);
        }
    }
}
