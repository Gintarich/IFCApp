using IFCApp.TeklaServices.Utils;
using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Geometry3d;
using IFCApp.Core.Geometry;
using Tekla.Structures;
using TS = Tekla.Structures.Model;
using Tekla.Structures.Model;
using System.Data;
using Tekla.Structures.ModelInternal;
using IFCApp.TeklaServices.Services;
using System.Diagnostics;

namespace IFCApp.TeklaServices;

public class TeklaOpeningMaker
{
    private TS.Component _openingComponent;
    private TeklaWindowConfig _windowConfig;
    private readonly TeklaDoorConfig _teklaDoorConfig;

    public List<Wall> Walls { get; }
    public TS.Model Model { get; }

    public TeklaOpeningMaker(List<Wall> walls, TeklaWindowConfig windowConfig, TeklaDoorConfig teklaDoorConfig)
    {
        Walls = walls;
        Model = new TS.Model();
        _windowConfig = windowConfig;
        _teklaDoorConfig = teklaDoorConfig;
    }

    public void GenerateOpenings()
    {
        Clear();
        Model.CommitChanges();
        foreach (Wall wall in Walls)
        {
            _openingComponent = _windowConfig.GetConfig(wall);
            List<Window> windows = wall.GetWindows();
            foreach (Window window in windows)
            {
                InsertWindow(window);
                if (wall is SandwichPanel sp && sp.LayerCount>3)
                {
                    CutOutLastLayer(window);
                }
            }
            _openingComponent = _teklaDoorConfig.GetConfig(wall);
            List<Door> doors = wall.GetDoors();
            foreach (Door door in doors)
            {
                InsertWindow(door);
                if (wall is SandwichPanel sp && sp.LayerCount>3)
                {
                    CutOutLastLayer(door);
                }
            }
        }
        Model.CommitChanges();
    }

    private void CutOutLastLayer(Opening opening)
    {
        Point startPoint = opening.GetStartPoint().TeklaPoint();
        Point endPoint = opening.GetEndPoint().TeklaPoint();
        Identifier identifier = new Identifier(opening.FatherID);
        var father = Model.SelectModelObject(identifier) as TS.Beam;
        if (father is null) return;
        var assembly = father.GetAssembly();
        var children = assembly.GetSecondaries().ToList<Part>();
        var parts = children.Where(x => x is Part).Cast<Part>().ToList();
        var kiegelis = parts.Where(x => x.Name == "APDARES ĶIEĢELIS");
        var finishLayer = kiegelis.OrderByDescending(x => x.GetDoubleProp("VOLUME")).FirstOrDefault();
        //var finishLayer = assembly
        //     .GetSecondaries().ToList<Part>()
        //     .Where(x => x.Name == "APDARES ĶIEĢELIS")
        //     .OrderByDescending(x => x.GetDoubleProp("VOLUME"))
        //     .FirstOrDefault();
        if (finishLayer is null) return;
        var cs = finishLayer.GetCoordinateSystem();
        var matrix = new TransformationPlane(cs);
        var local = matrix.TransformationMatrixToLocal;
        var global = matrix.TransformationMatrixToGlobal;
        var localSp = local.Transform(startPoint);
        var localEp = local.Transform(endPoint);
        if (localSp.Y > localEp.Y)
        {
            // Swap to kepp sp lower
            var tmp = localSp;
            localSp = localEp;
            localEp = tmp;
        }
        if (localSp.X > localEp.X)
        {
            var tmp = localSp.X;
            localSp.X = localEp.X;
            localEp.X = tmp;
        }
        var pts = new List<Point>
        {
            new Point(localSp.X+30, localSp.Y-20, 0),
            new Point(localEp.X-30, localSp.Y-20, 0),
            new Point(localEp.X-30, localEp.Y-30, 0),
            new Point(localSp.X+30, localEp.Y-30, 0)
        };
        pts = pts.Select(p => global.Transform(p)).ToList(); //TransformPoints
        BooleanPart bp = new BooleanPart();
        bp.Father = finishLayer;
        var cp = new ContourPlate();
        pts.ForEach(x => cp.AddContourPoint(new ContourPoint(x, new Chamfer())));
        cp.Name = "Cutout";
        cp.Profile.ProfileString = "100";
        cp.Class = BooleanPart.BooleanOperativeClassName;
        cp.Insert();
        cp.SetUserProperty("Inserted", 1);
        bp.SetOperativePart(cp);
        bp.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
        bp.Insert();
        bp.OperativePart.SetUserProperty("Inserted", 1);
        cp.Delete();
    }

    private void InsertWindow(Opening opening)
    {
        Point startPoint = opening.GetStartPoint().TeklaPoint();
        Point endPoint = opening.GetEndPoint().TeklaPoint();
        Identifier identifier = new Identifier(opening.FatherID);
        var father = Model.SelectModelObject(identifier) as TS.Beam;
        if (father == null) return;

        //var mainpart = father.GetChildren()
        //    .ToList()
        //    .Where(x => x is TS.Beam b)
        //    .Cast<TS.Beam>()
        //    .Where(x => x.Name == "NESOŠAIS SLĀNIS").First();

        //if (mainpart == null) return;

        TS.ComponentInput input = new TS.ComponentInput();
        input.AddInputObject(father);
        input.AddOneInputPosition(startPoint);
        input.AddOneInputPosition(endPoint);
        _openingComponent.SetComponentInput(input);
        _openingComponent.Insert();
        _openingComponent.SetUserProperty("Inserted", 1);
        _openingComponent.Modify();
    }

    private void Clear()
    {
        var selector = Model.GetModelObjectSelector();
        var components = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.COMPONENT)
            .ToList().Cast<Component>();
        var compOpenings = components.
            Where(x => x.Name == "SandwichWallWindow" && x.IsUserCreated()).ToList();
        foreach (var opening in compOpenings)
        {
            opening.Delete();
        }
        var openings = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BOOLEANPART)
            .ToList().Cast<BooleanPart>();
        var openingsToDelete = openings.Where(x => x.OperativePart.IsUserCreated());
        foreach(var opening in openingsToDelete)
        {
            opening.Delete();
        }
    }
}
