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
    private Action<List<double>, Guid, Guid> _func;

    public DowelComponent() { }
    public DowelComponent(Guid firstPart, Guid secondPart, Action<List<double>,Guid,Guid> func)
    {
        FirstPart = firstPart;
        SecondPart = secondPart;
        _func = func;
    }

    public void Run(Model model)
    {
        model.TryGetValue(FirstPart, out var first);
        model.TryGetValue(SecondPart, out var second);
        if (first is null || second is null) throw new Exception("Dowel component cannot operate on null elements, check if element is in Core model");
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
        var locations = CalculateLocations(length, step);
        _func(locations,FirstPart,SecondPart);
    }
    private List<double> CalculateLocations(double length, double step)
    {
        double whenToCreate = 300;
        if (length < whenToCreate) return [];
        //TODO: Solve edge cases :
        //      3)Thereis space for two dowels
        int stepCount = (int)Math.Ceiling(length / step);
        if (stepCount <= 1) return [Math.Round((length / 2) / 10) * 10];
        var realStep = length / stepCount;
        var firstStep = Math.Ceiling(realStep / 10) * 10;
        var lastStep = firstStep;
        if (stepCount == 2) return [firstStep, length-lastStep];
        var midLength = length - (lastStep + firstStep);
        List<double> midlocations = CalculateMiddleSection(midLength, step);
        return Accumulate(firstStep, midlocations);
    }
    private List<double> CalculateMiddleSection(double length, double step)
    {
        int midStepCount = (int)Math.Ceiling(length / step);
        if (midStepCount <= 1) return [length];

        var realStep = length / midStepCount;
        var interval = Math.Round(realStep / 10) * 10;
        if (midStepCount == 2) return [interval, length - interval];

        List<double> intervals = [];
        for (var i = 0; i < midStepCount - 1; i++)
        {
            intervals.Add(interval);
        }
        intervals.Add(length - (interval * (midStepCount - 1)));

        return intervals;
    }
    private List<double> Accumulate(double first, List<double> middleSection)
    {
        if (middleSection.Count == 0) return [first];
        if (middleSection.Count == 1) return [first, first + middleSection[0]];
        List<double> intervals = [first];
        double currentVal = first;
        var count = middleSection.Count;
        for (var i = 0; i < count; i++)
        {
            currentVal = currentVal + middleSection[i];
            intervals.Add(currentVal);
        }
        return intervals;
    }
}
