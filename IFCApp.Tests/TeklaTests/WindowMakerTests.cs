using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
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
            TeklaDoorConfig dCfng = new TeklaDoorConfig();
            TeklaWindowConfig wCfig = new TeklaWindowConfig();
            List<Wall> walls = new List<Wall>()
            {
                new Wall(new BBox([new Point3d(50000,-400,0), new Point3d(80000,400,4000)]),new Matrix4d())
                    .TryToAddOpening(new Opening(new Point3d(65000,0,1000),new Point3d(67000,0,3000),"dcd1e75e-2e1e-4668-8bdc-1278a6d27d71"))
                    .TryToAddOpening(new Opening(new Point3d(70000,0,1000),new Point3d(72000,0,3000),"dcd1e75e-2e1e-4668-8bdc-1278a6d27d71"))
            };

            var windowMaker = new TeklaOpeningMaker(walls,wCfig,dCfng);
            windowMaker.GenerateOpenings();
        }

        [TestMethod]
        public void AddAllWindows()
        {
            //Dependencies
            BBoxService bBoxService = new BBoxService();
            TransformationService transformationService = new TransformationService(VUGDCoordinateSystems.InverseBol);
            TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
            TeklaDoorConfig dCfng = new TeklaDoorConfig();
            TeklaWindowConfig wCfig = new TeklaWindowConfig();
            //Script

            //Get Windows
            IFCModel model = new IFCModel(@"Z:\BCD projekti\Eduards Beernaerds_7AM\VUGD Depo kuldiga\Teklas modeli\KUL-7AM-00-00-M3-BK-0001\Automation\KUL-7AM-00-00-M3-AR-0001.ifc");
            IfcWindowService winServ = new IfcWindowService(model, transformationService, bBoxService);
            var windows = winServ.GetWindows();

            //Get Walls
            List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["SIENAS PANELIS"]);

            //Add Windows to walls
            foreach (Wall wall in walls)
            {
                foreach (var window in windows)
                {
                    wall.TryToAddOpening(window);
                }
            }
            TeklaOpeningMaker wm = new TeklaOpeningMaker(walls, wCfig, dCfng);
            wm.GenerateOpenings();
        }
    }
}
