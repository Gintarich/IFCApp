using IFCApp.Core;
using IFCApp.Core.Elements;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tekla.Structures.Model.UI;

namespace IFCApp.Tests.TeklaTests;

[TestClass]
public class OpeningMakerTests
{
    [TestMethod]
    public void ClearOpenings()
    {
        TeklaDoorConfig dCfng = new TeklaDoorConfig();
        TeklaWindowConfig wCfig = new TeklaWindowConfig();
        var maker = new TeklaOpeningMaker([], wCfig, dCfng);
        maker.ClearAllOpenings();
    }

    [TestMethod]
    public void AddHvacOpenings()
    {
        TransformationService transformationService1 = new TransformationService(VUGDCoordinateSystems.InverseBol);
        BBoxService bBoxServ = new BBoxService();
        TeklaBoundingBoxService tBoxServ = new TeklaBoundingBoxService();
        TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
        Model gbModel = LoadModel("BOL-7AM-00-00-M3-BK-0001");
        TeklaWallService wServ = new TeklaWallService(tBoxServ);
        var model = new IFCModel(GetIfcPath("BOL-7AM-00-00-M3-AR-0001"), transformationService1, bBoxServ);
        var walls = gbModel.Elements.Where(e => e is Wall).Cast<Wall>().ToList();
        var openings = model.GetHvacOpenings(["ATV"]);

        foreach (var wall in walls)
        {
            foreach(var opening in openings)
            {
                wall.TryToAddOpening(opening,0.000001);
            }
        }

        List<HvacOpening> hvacOpenings = new List<HvacOpening>();
        foreach(var wall in walls)
        {
            var hopen = wall.GetHvacOpenings();
            if(hopen.Count > 0)
            {
                hvacOpenings.AddRange(hopen);
            }
        }

        TeklaHvacOpeningMaker hvacOMaker = new TeklaHvacOpeningMaker(walls);
        hvacOMaker.ClearAllOpenings();
        hvacOMaker.GenerateOpenings();

        //foreach (var opening in hvacOpenings)
        //{
        //    if (opening.IsCircle)
        //    {
        //        var startPoint = opening.GetStartPoint().TeklaPoint();
        //        var endPoint = opening.GetEndPoint().TeklaPoint();
        //        var diameter = opening.GetDiameter();
        //        gd.DrawCylinder(startPoint, endPoint, diameter);
        //    }
        //    else
        //    {
        //        gd.DrawBox(opening.GetBox());
        //    }
        //}
        //walls.ForEach(wall => gd.DrawBox(wall.GetBox(), new Color(0, 0.5, 0.5)));
    }

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
        IFCModel model = new IFCModel(@"Z:\BCD projekti\Eduards Beernaerds_7AM\VUGD Depo kuldiga\Teklas modeli\KUL-7AM-00-00-M3-BK-0001\Automation\KUL-7AM-00-00-M3-AR-0001.ifc");
        IfcDoorService doorServ = new IfcDoorService(model, transformationService, bBoxService);
        var doors = doorServ.GetDoors();

        IfcWindowService windowService = new IfcWindowService(model, transformationService, bBoxService);
        var windows = windowService.GetWindows();

        //Get Walls
        List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["TRĪSSLĀŅU SIENAS PANELIS", "VIENSLĀŅU SIENAS PANELIS"]);

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
        TeklaOpeningMaker wm = new TeklaOpeningMaker(walls, wCfig, dCfng);
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
        List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["TRĪSSLĀŅU SIENAS PANELIS", "VIENSLĀŅU SIENAS PANELIS", "MŪRA SIENA"]);

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
        TeklaOpeningMaker wm = new TeklaOpeningMaker(walls, wCfig, dCfng);
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
        List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["TRĪSSLĀŅU SIENAS PANELIS", "VIENSLĀŅU SIENAS PANELIS", "MŪRA SIENA"]);

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
        TeklaOpeningMaker wm = new TeklaOpeningMaker(walls, wCfig, dCfng);
        wm.GenerateOpenings();
    }
    public Model LoadModel(string name)
    {
        Model model = new Model();
        string path = Path.Combine(GetFolderPath(), name + ".json");
        var json = File.ReadAllText(path);
        if (!string.IsNullOrEmpty(json))
        {
            model = JsonSerializer.Deserialize<Model>(json);
        }
        return model;
    }
    public string GetFolderPath()
    {
        return Path.Combine(new ModelAttributeServer().GetFilePath(), "Automation");
    }
    public string GetIfcPath(string name)
    {
        return Path.Combine(GetFolderPath(),$"{name}.ifc");
    }
}

 