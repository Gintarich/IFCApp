using IFCApp.Core;
using IFCApp.Core.Elements;
using IFCApp.Core.Services;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.Tests.TeklaTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void MustSerialize()
        {
            var model = GenerateModel();
            JsonSerializationService jsonService = new JsonSerializationService(
                @"Z:\BCD projekti\Eduards Beernaerds_7AM\VUGD Depo Bolderaja\Teklas modeli\BOL-7AM-00-00-M3-BK-0001");
            jsonService.Write(model);
            jsonService.Read();
        }

        private Model GenerateModel()
        {
            //Dependencies
            BBoxService bBoxService = new BBoxService();
            TransformationService transformationService = new TransformationService(VUGDCoordinateSystems.InverseBol);
            TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
            TeklaDoorConfig dCfng = new TeklaDoorConfig();
            TeklaWindowConfig wCfig = new TeklaWindowConfig();
            //Script

            //Get Windows
            IFCModel model = new IFCModel("BOL-7AM-00-00-M3-AR-0001.ifc");
            IfcDoorService doorServ = new IfcDoorService(model, transformationService, bBoxService);
            var doors = doorServ.GetDoors();

            IfcWindowService windowService = new IfcWindowService(model, transformationService, bBoxService);
            var windows = windowService.GetWindows();

            //Get Walls
            List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["SIENAS PANELIS", "MŪRA SIENA"]);

            //Add doors to walls
            foreach (Wall wall in walls)
            {
                foreach (var door in doors)
                {
                    wall.TryToAddOpening(door);
                }
                foreach (var win in windows)
                {
                    wall.TryToAddOpening(win);
                }
            }
            var myModel = new Model();
            myModel.Insert(walls);
            return myModel;
        }
    }
}
