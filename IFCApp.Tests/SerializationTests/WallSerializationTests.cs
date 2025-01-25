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
        private const double Tolerance = 0.001;

        [TestMethod]
        public void SerializeWall_ShouldReturnCorrectJson()
        {
            // Arrange
            var wall = new Wall(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) }));
            var expectedJson = "{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";

            // Act
            var json = JsonSerializer.Serialize<Wall>(wall);

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeWall_ShouldReturnCorrectWall()
        {
            // Arrange
            var json = "{\"$type\":\"Wall\",\"TeklaIdentifier\":1,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";
            var expectedBox = new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) });

            // Act
            var wall = JsonSerializer.Deserialize<Wall>(json);

            // Assert
            Assert.IsNotNull(wall);
            AssertHelpers.AssertBBoxAreEqual(expectedBox, wall.Box, Tolerance);
        }

        [TestMethod]
        public void SerializeSandwichPanel_ShouldReturnCorrectJson()
        {
            // Arrange
            var sandwichPanel = new SandwichPanel(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) }), new Layers(70, 150, 250));
            var expectedJson = "{\"$type\":\"SandwichPanel\",\"Layers\":{\"InnerLayerThickness\":70,\"OuterLayerThickness\":150,\"InsulationThickness\":250},\"LayerCount\":0,\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";

            // Act
            var json = JsonSerializer.Serialize<Wall>(sandwichPanel);

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeSandwichPanel_ShouldReturnCorrectSandwichPanel()
        {
            // Arrange
            var json = "{\"$type\":\"SandwichPanel\",\"Layers\":{\"InnerLayerThickness\":70,\"OuterLayerThickness\":150,\"InsulationThickness\":250},\"LayerCount\":0,\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";
            var expectedBox = new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) });

            // Act
            var sandwichPanel = JsonSerializer.Deserialize<Wall>(json);

            // Assert
            Assert.IsNotNull(sandwichPanel);
            AssertHelpers.AssertBBoxAreEqual(expectedBox, sandwichPanel.Box, Tolerance);
        }

        [TestMethod]
        public void SerializeWallPanel_ShouldReturnCorrectJson()
        {
            // Arrange
            var wallPanel = new WallPanel(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) }));
            var expectedJson = "{\"$type\":\"WallPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";

            // Act
            var json = JsonSerializer.Serialize<Wall>(wallPanel);

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeWallPanel_ShouldReturnCorrectWallPanel()
        {
            // Arrange
            var json = "{\"$type\":\"WallPanel\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}";
            var expectedBox = new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) });

            // Act
            var wallPanel = JsonSerializer.Deserialize<Wall>(json) as WallPanel;

            // Assert
            Assert.IsNotNull(wallPanel);
            AssertHelpers.AssertBBoxAreEqual(expectedBox, wallPanel.Box, Tolerance);
        }

        [TestMethod]
        public void SerializeWallList_ShouldReturnCorrectJson()
        {
            // Arrange
            var walls = new List<Wall>
            {
                new Wall(new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) })),
                new Wall(new BBox(new List<Point3d> { new Point3d(1, 1, 1), new Point3d(2, 2, 2) }))
            };
            var expectedJson = "[{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":1,\"Y\":1,\"Z\":1},\"Max\":{\"X\":2,\"Y\":2,\"Z\":2},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}]";

            // Act
            var json = JsonSerializer.Serialize<List<Wall>>(walls);

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeWallList_ShouldReturnCorrectWallList()
        {
            // Arrange
            var json = "[{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0},\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},{\"$type\":\"Wall\",\"TeklaIdentifier\":0,\"ShouldHaveOpening\":false,\"Openings\":[],\"Box\":{\"Min\":{\"X\":1,\"Y\":1,\"Z\":1},\"Max\":{\"X\":2,\"Y\":2,\"Z\":2},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}},\"Matrix\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}]";
            var expectedBox1 = new BBox(new List<Point3d> { new Point3d(0, 0, 0), new Point3d(1, 1, 1) });
            var expectedBox2 = new BBox(new List<Point3d> { new Point3d(1, 1, 1), new Point3d(2, 2, 2) });

            // Act
            var walls = JsonSerializer.Deserialize<List<Wall>>(json);

            // Assert
            Assert.IsNotNull(walls);
            Assert.AreEqual(2, walls.Count);
            AssertHelpers.AssertBBoxAreEqual(expectedBox1, walls[0].Box, Tolerance);
            AssertHelpers.AssertBBoxAreEqual(expectedBox2, walls[1].Box, Tolerance);
        }
    }
}
