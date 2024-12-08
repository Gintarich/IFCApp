using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Services;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Model;
using TS = Tekla.Structures.Model;

namespace IFCApp.TeklaServices;

public class TeklaWallService
{
    private TeklaBoundingBoxService _boundingBoxService;

    public TeklaWallService(TeklaBoundingBoxService boundingBoxService)
    {
        _boundingBoxService = boundingBoxService;
    }

    public List<Wall> GetWalls(string AssemblyName)
    {
        var model = new Model();
        var selector = model.GetModelObjectSelector();
        var allAssemblies = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY)
            .ToList().Cast<TS.Assembly>();
        TeklaBoundingBoxService bbService = _boundingBoxService;
        var teklaWalls = allAssemblies.Where(x => x.Name == AssemblyName)
            .Select(x => x.GetMainPart()).Cast<TS.Beam>().ToList();

        List<Wall> walls = new List<Wall>();
        foreach (var teklaWall in teklaWalls)
        {
            var box = bbService.GetBox(teklaWall);
            var id = teklaWall.Identifier.ID;
            var wall = new Wall(box);
            wall.TeklaIdentifier = id;
            walls.Add(wall);
        }
        return walls;
    }
    public List<Wall> GetWalls(List<string> AssemblyNames)
    {
        List<Wall> walls = new List<Wall>();
        var model = new Model();
        var selector = model.GetModelObjectSelector();
        var allAssemblies = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY)
            .ToList().Cast<TS.Assembly>();
        TeklaBoundingBoxService bbService = _boundingBoxService;
        var panels = allAssemblies.Where(x => AssemblyNames.Contains(x.Name))
           .Cast<TS.Assembly>();
        foreach (var panel in panels)
        {
            TeklaLayerService teklaLayerService = new TeklaLayerService(panel);
            var mainPart = panel.GetMainPart() as TS.Beam;
            if (teklaLayerService.GetLayerCount()>1)
            {
                var layers = teklaLayerService.GetLayers();
                var box = _boundingBoxService.GetBox(mainPart);
                var sandwichPanel = new SandwichPanel(box, layers);
                sandwichPanel.TeklaIdentifier = mainPart.Identifier.ID;
                walls.Add(sandwichPanel);
            }
            else
            {
                var wallPanel = new WallPanel(_boundingBoxService.GetBox(mainPart));
                wallPanel.TeklaIdentifier = mainPart.Identifier.ID;
                walls.Add(wallPanel);
            }
        }
        return walls;
    }
    public Wall GetWall(int id)
    {
        var model = new Model();
        var mo = model.SelectModelObject(new Identifier(id));
        var beam = mo as TS.Beam;
        BBox box = _boundingBoxService.GetBox(beam);
        Wall wall = new Wall(box);
        return wall;
    }
}
