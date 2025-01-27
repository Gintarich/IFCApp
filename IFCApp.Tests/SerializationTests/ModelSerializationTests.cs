using IFCApp.Core;
using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace IFCApp.Tests
{
    [TestClass]
    public class ModelSerializationTests
    {
        [TestMethod]
        public void TestModelSerialization()
        {
            // Arrange
            var model = new Model
            {
                ModelName = "TestModel",
                ModelPath = "C:/Models/TestModel.ifc",
                BBoxes = new Dictionary<string, BBox>
                {
                    { "Box1", new BBox { Min = new Point3d(0, 0, 0), Max = new Point3d(1, 1, 1), 
                        CS = new Matrix4d(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }) } }
                },
                CS = new Matrix4d(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }),
                Elements = new List<ElementBase> { new ElementBase() }
            };

            // Act
            var json = JsonSerializer.Serialize(model);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("\"ModelName\":\"TestModel\""));
            Assert.IsTrue(json.Contains("\"ModelPath\":\"C:/Models/TestModel.ifc\""));
            Assert.IsTrue(json.Contains("\"BBoxes\":{\"Box1\":{\"Min\":{\"X\":0,\"Y\":0,\"Z\":0}," +
                "\"Max\":{\"X\":1,\"Y\":1,\"Z\":1},\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}}}"));
            Assert.IsTrue(json.Contains("\"CS\":{\"Matrix\":[[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]}"));
        }
    }
}
