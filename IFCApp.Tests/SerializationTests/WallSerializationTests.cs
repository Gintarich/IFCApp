using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace IFCApp.Tests.SerializationTests
{
    [TestClass]
    public class WallSerializationTests
    {
        private const double Tolerance = 1e-9;

        [TestMethod]
        public void SerializeWall_ShouldReturnCorrectJson()
        {
            // Arrange
            var box = new BBox(new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(300, 200, 500)
            });
            var wall = new Wall(box);

            // Act
            var json = JsonSerializer.Serialize(wall);

            // Assert
            var expectedJson = "{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeWall_ShouldReturnCorrectWall()
        {
            // Arrange
            var json = "{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";

            // Act
            var wall = JsonSerializer.Deserialize<Wall>(json);

            // Assert
            var expectedBox = new BBox(new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(300, 200, 500)
            });

            AssertHelpers.AssertBBoxAreEqual(expectedBox, wall.Box, Tolerance);
        }

        [TestMethod]
        public void SerializeSandwichPanel_ShouldReturnCorrectJson()
        {
            // Arrange
            var box = new BBox(new List<Point3d>
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            });
            var layers = new Layers(); // Assuming Layers is a class with a default constructor
            var sandwichPanel = new SandwichPanel(box, layers);

            // Act
            var json = JsonSerializer.Serialize(sandwichPanel);

            // Assert
            var expectedJson = "{\"$type\":\"SandwichPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-100,\"Y\":-100,\"Z\":-100},\"Max\":{\"X\":100,\"Y\":100,\"Z\":100},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]},\"Layers\":null,\"LayerCount\":0}";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeSandwichPanel_ShouldReturnCorrectSandwichPanel()
        {
            // Arrange
            var json = "{\"$type\":\"SandwichPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-100,\"Y\":-100,\"Z\":-100},\"Max\":{\"X\":100,\"Y\":100,\"Z\":100},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]},\"Layers\":null,\"LayerCount\":0}";

            // Act
            var sandwichPanel = JsonSerializer.Deserialize<SandwichPanel>(json);

            // Assert
            var expectedBox = new BBox(new List<Point3d>
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            });

            AssertHelpers.AssertBBoxAreEqual(expectedBox, sandwichPanel.Box, Tolerance);
        }

        [TestMethod]
        public void SerializeWallPanel_ShouldReturnCorrectJson()
        {
            // Arrange
            var box = new BBox(new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(300, 200, 500)
            });
            var wallPanel = new WallPanel(box);

            // Act
            var json = JsonSerializer.Serialize(wallPanel);

            // Assert
            var expectedJson = "{\"$type\":\"WallPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeWallPanel_ShouldReturnCorrectWallPanel()
        {
            // Arrange
            var json = "{\"$type\":\"WallPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";

            // Act
            var wallPanel = JsonSerializer.Deserialize<WallPanel>(json);

            // Assert
            var expectedBox = new BBox(new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(300, 200, 500)
            });

            AssertHelpers.AssertBBoxAreEqual(expectedBox, wallPanel.Box, Tolerance);
        }

        [TestMethod]
        public void SerializeWallList_ShouldReturnCorrectJson()
        {
            // Arrange
            var box1 = new BBox(new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(300, 200, 500)
            });
            var box2 = new BBox(new List<Point3d>
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            });
            var layers = new Layers(); // Assuming Layers is a class with a default constructor

            var wallPanel = new WallPanel(box1);
            var sandwichPanel = new SandwichPanel(box2, layers);

            var walls = new List<Wall> { wallPanel, sandwichPanel };

            // Act
            var json = JsonSerializer.Serialize(walls);

            // Assert
            var expectedJson = "[{\"$type\":\"WallPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},{\"$type\":\"SandwichPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-100,\"Y\":-100,\"Z\":-100},\"Max\":{\"X\":100,\"Y\":100,\"Z\":100},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]},\"Layers\":null,\"LayerCount\":0}]";
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeWallList_ShouldReturnCorrectWallList()
        {
            // Arrange
            var json = "[{\"$type\":\"WallPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-300,\"Y\":-200,\"Z\":-500},\"Max\":{\"X\":300,\"Y\":200,\"Z\":500},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},{\"$type\":\"SandwichPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":-100,\"Y\":-100,\"Z\":-100},\"Max\":{\"X\":100,\"Y\":100,\"Z\":100},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]},\"Layers\":null,\"LayerCount\":0}]";

            // Act
            var walls = JsonSerializer.Deserialize<List<Wall>>(json);

            // Assert
            Assert.AreEqual(2, walls.Count);
            Assert.IsInstanceOfType(walls[0], typeof(WallPanel));
            Assert.IsInstanceOfType(walls[1], typeof(SandwichPanel));

            var expectedBox1 = new BBox(new List<Point3d>
            {
                new Point3d(-300, -200, -500),
                new Point3d(300, 200, 500)
            });
            var expectedBox2 = new BBox(new List<Point3d>
            {
                new Point3d(-100, -100, -100),
                new Point3d(100, 100, 100)
            });

            AssertHelpers.AssertBBoxAreEqual(expectedBox1, walls[0].Box, Tolerance);
            AssertHelpers.AssertBBoxAreEqual(expectedBox2, walls[1].Box, Tolerance);
        }
    }
}
