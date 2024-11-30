using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Utils;
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
                new Wall(new BBox([new Point3d(50000,-400,0), new Point3d(80000,400,4000)]),new Matrix4d())
                    .TryToAddWindow(new Window(new Point3d(65000,0,1000),new Point3d(67000,0,3000),1047717))
                    .TryToAddWindow(new Window(new Point3d(70000,0,1000),new Point3d(72000,0,3000),1047717))
            };

            var windowMaker = new TeklaWindowMaker(walls);
            windowMaker.GenerateOpenings();
        }

        [TestMethod]
        public void AddAllWindows()
        {
            IFCModel model = new IFCModel();
            IfcWindowService winServ = new IfcWindowService(model);
            var windows = winServ.GetWindows();
            List<Wall> walls = new TeklaWallService().GetWalls("SIENAS PANELIS");
            //var wall = wallMaker.GetWall(1056597);
            //wall.TeklaIdentifier = 1056597;
            //TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
            //gd.DrawBox(wall.GetBox());
            foreach (Wall wall in walls)
            {
                foreach (var window in windows)
                {
                    wall.TryToAddWindow(window);
                }
            }
            TeklaWindowMaker wm = new TeklaWindowMaker(walls); 
            wm.GenerateOpenings();
        }
    }
}
