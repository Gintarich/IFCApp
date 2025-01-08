using IFCApp.Core.Elements;
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

namespace IFCApp.Tests.TeklaTests;

[TestClass]
public class OpeningMakerTests
{
    [TestMethod]
    public void AddAllOpenings()
    {
        //Dependencies
        BBoxService bBoxService = new BBoxService();
        TransformationService transformationService = new TransformationService(VUGDCoordinateSystems.InverseKul);
        TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
        TeklaDoorConfig dCfng = new TeklaDoorConfig();
        TeklaWindowConfig wCfig = new TeklaWindowConfig();
        //Script

        //Get Windows
        IFCModel model = new IFCModel();
        IfcDoorService doorServ = new IfcDoorService(model, transformationService, bBoxService);
        var doors = doorServ.GetDoors();

        IfcWindowService windowService = new IfcWindowService(model, transformationService, bBoxService);
        var windows = windowService.GetWindows();

        //Get Walls
        List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["SIENAS PANELIS"]);

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
        TeklaOpeningMaker wm = new TeklaOpeningMaker(walls,wCfig,dCfng);
        wm.GenerateOpenings();
    }
    [TestMethod]
    public void AddAllOpeningsBOL()
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
        TeklaOpeningMaker wm = new TeklaOpeningMaker(walls,wCfig,dCfng);
        wm.GenerateOpenings();
    }
    [TestMethod]
    public void AddAllOpeningsDZI()
    {
        //Dependencies
        BBoxService bBoxService = new BBoxService();
        TransformationService transformationService = new TransformationService(VUGDCoordinateSystems.InverseDzin);
        TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
        TeklaDoorConfig dCfng = new TeklaDoorConfig();
        TeklaWindowConfig wCfig = new TeklaWindowConfig();
        //Script

        //Get Windows
        IFCModel model = new IFCModel("DZI-7AM-00-00-M3-AR-0001.ifc");
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
        TeklaOpeningMaker wm = new TeklaOpeningMaker(walls,wCfig,dCfng);
        wm.GenerateOpenings();
    }
}
