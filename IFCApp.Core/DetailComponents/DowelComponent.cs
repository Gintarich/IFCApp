using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.DetailComponents;
public class DowelComponent
{
    public Guid FirstPart { get; set; }
    public Guid SecondPart { get; set; }

    public DowelComponent() { }
    public DowelComponent(Guid firstPart, Guid secondPart)
    {
        FirstPart = firstPart;
        SecondPart = secondPart;
    }

    public void Run(Model model)
    {
        model.TryGetValue(FirstPart, out var first);
        model.TryGetValue(SecondPart, out var second);
        if (first is Slab slab && second is Wall wall) AddDowelToSlabAndWall(slab, wall);
        else if (first is Wall && second is Wall) AddDowelToWallAndWall(first, second);
        else { throw new NotImplementedException($"Dowel Component is not ment to be used with {first.GetType().ToString()} and {second.GetType().ToString()}"); }
    }

    private void AddDowelToWallAndWall(ElementBase first, ElementBase second)
    {
        throw new NotImplementedException();
    }

    private void AddDowelToSlabAndWall(Slab slab, Wall wall)
    {
        //Parameters 
        var step = 1500;

        List<Door> doors = wall.Openings.Where(x => x is Door).Cast<Door>().ToList();

        var min = wall.Box.Min;
        var max = wall.Box.Max;
        var midWidth = (wall.Box.Max.Y + wall.Box.Min.Y) / 2;
        var startPoint = new Point3d(min.X, midWidth, min.Z);
        var endPoint = new Point3d(max.X, midWidth, min.Z);
        var length = max.X > min.X ? max.X - min.X : min.X - max.X;
    }
}
