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
            }
            _openingComponent = _teklaDoorConfig.GetConfig(wall);
            List<Door> doors = wall.GetDoors();
            foreach (Door door in doors)
            {
                InsertWindow(door);
            }
        }
        Model.CommitChanges();
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
        var openings = components.
            Where(x => x.Name == "SandwichWallWindow" && x.IsUserCreated()).ToList();
        foreach (var opening in openings)
        {
            opening.Delete();
        }
    }
}
