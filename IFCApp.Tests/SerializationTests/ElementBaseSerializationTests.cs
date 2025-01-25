using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace IFCApp.Tests.SerializationTests
{
    [TestClass]
    public class ElementBaseSerializationTests
    {
        private const double Tolerance = 0.001;

        [TestMethod]
        public void SerializeAndDeserializeElementBaseList_ShouldReturnCorrectList()
        {
            // Arrange
            var elements = new List<ElementBase>
            {
                new Wall(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) })),
                new SandwichPanel(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) }), new Layers(70, 150, 250)),
                new WallPanel(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) })),
                new Door(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 2, 1) })),
                new Window(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) })),
                new Opening(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) })),
                new Beam { StartPoint = new Point3d(0, 0, 0), EndPoint = new Point3d(1, 1, 1) }
            };

            var options = new JsonSerializerOptions
            {
                //WriteIndented = true
            };

            // Act
            var json = JsonSerializer.Serialize(elements, options);
            var deserializedElements = JsonSerializer.Deserialize<List<ElementBase>>(json, options);

            // Assert
            Assert.IsNotNull(deserializedElements);
            Assert.AreEqual(elements.Count, deserializedElements.Count);

            for (int i = 0; i < elements.Count; i++)
            {
                Assert.AreEqual(elements[i].GetType(), deserializedElements[i].GetType());
            }
        }
    }
}
