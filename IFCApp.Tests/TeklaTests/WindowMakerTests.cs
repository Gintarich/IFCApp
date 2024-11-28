using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.Tests.TeklaTests
{
    [TestClass]
    public class WindowMakerTests
    {
        [TestMethod]
        public void TestIfWindowsGetsCreated()
        {
            List<Wall> walls = new List<Wall>()
            {
                new Wall(new BBox(new List<Point3d>()),new Matrix4d())
                    .AddWindow(new Window(new Point3d(65000,0,1000),new Point3d(67000,0,3000),629267))
                    .AddWindow(new Window(new Point3d(70000,0,1000),new Point3d(72000,0,3000),629267))
            };

            var windowMaker = new TeklaWindowMaker(walls);
            windowMaker.GenerateOpenings();
        }
    }
}
