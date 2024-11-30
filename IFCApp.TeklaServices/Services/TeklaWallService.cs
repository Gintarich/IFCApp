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
    public List<Wall> GetWalls(string AssemblyName)
    {
        var model = new Model();
        var selector = model.GetModelObjectSelector();
        var allAssemblies = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY)
            .ToList().Cast<TS.Assembly>();
        TeklaBoundingBoxService bbService = new TeklaBoundingBoxService();
        var teklaWalls = allAssemblies.Where(x => x.Name == AssemblyName)
            .Select(x=>x.GetMainPart()).Cast<TS.Beam>().ToList();

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
    public Wall GetWall(int id)
    {
        var model = new Model();
        var mo = model.SelectModelObject(new Identifier(id));
        var beam = mo as TS.Beam;
        BBox box = new TeklaBoundingBoxService().GetBox(beam);
        Wall wall = new Wall(box);
        return wall;
    }
}
