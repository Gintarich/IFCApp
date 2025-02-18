using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.DetailComponents;
public class DowelComponent : Component
{
    public DowelComponent() { }
    public DowelComponent(Guid firstPart, Guid secondPart)
    {
        FirstPart = firstPart;
        SecondPart = secondPart;
    }

    public override void Run(Model model, Action<List<Point3d>, Guid, Guid> func)
    {
        model.TryGetValue(FirstPart, out var first);
        model.TryGetValue(SecondPart, out var second);
        if (first is null || second is null) throw new Exception("Dowel component cannot operate on null elements, check if element is in Core model");
        if (first is Slab slab && second is Wall wall) AddDowelToSlabAndWall(slab, wall, func);
        else if (first is Wall && second is Wall) AddDowelToWallAndWall(first, second, func);
        else { throw new NotImplementedException($"Dowel Component is not ment to be used with {first.GetType().ToString()} and {second.GetType().ToString()}"); }
    }

    private void AddDowelToWallAndWall(ElementBase first, ElementBase second, Action<List<Point3d>, Guid, Guid> func)
    {
        throw new NotImplementedException();
    }

    private void AddDowelToSlabAndWall(Slab slab, Wall wall, Action<List<Point3d>, Guid, Guid> func)
    {
        //Parameters 
        double step = 1200;
        double threshold = 200;
        double firstOffset = 400;

        List<Door> doors = wall.Openings.Where(x => x is Door).Cast<Door>().ToList();
        List<Domain> domains = wall.GetLowerDomains(threshold);
        List<Point3d> points = CalcPoints(domains, step, wall, firstOffset);
        points = points.Select(x => wall.Box.CS.Apply(x)).ToList();
        func(points, FirstPart, SecondPart);
    }

    private List<Point3d> CalcPoints(List<Domain> domains, double step, Wall wall, double firstOffset)
    {
        var points = new List<Point3d>();
        foreach (Domain domain in domains)
        {
            points.AddRange(CalcPoints(domain, step, wall, firstOffset));
        }
        return points;
    }

    private IEnumerable<Point3d> CalcPoints(Domain domain, double step, Wall wall, double firstOffset)
    {
        List<Point3d> points = [];
        List<double> distances = CalculateLocations(domain, step, firstOffset);
        foreach (var dist in distances)
        {
            var midWidth = Math.Abs((wall.Box.Max.Y + wall.Box.Min.Y) / 2);
            points.Add(new Point3d(dist, midWidth, wall.Box.Min.Z));
        }
        return points;
    }

    private List<double> CalculateLocations(Domain domain, double step, double firstOffset)
    {
        var length = domain.Length;
        double whenToCreate = 300;
        if (length < whenToCreate) return [];
        //TODO: Solve edge cases :
        //      3)Thereis space for two dowels
        int stepCount = (int)Math.Ceiling(length / step);
        if (stepCount <= 1) return [Math.Round(domain.Mid / 10) * 10];
        var realStep = length / stepCount;
        var firstStep = firstOffset;
        var lastStep = firstStep;
        Domain midDomain = new Domain(domain.Start + firstStep, domain.End - lastStep);
        if (stepCount == 2) return [midDomain.Start, midDomain.End];
        List<double> locations = CalculateMiddleSection(midDomain, step);
        return locations;
    }
    private List<double> CalculateMiddleSection(Domain domain, double step)
    {
        var length = domain.Length;
        int midStepCount = (int)Math.Ceiling(length / step);
        if (midStepCount <= 1) return [domain.Start, domain.End];

        var realStep = length / midStepCount;
        if (midStepCount == 2) return [domain.Start, domain.Mid, domain.End];

        List<double> intervals = [];
        double current = domain.Start;
        for (var i = 0; i < midStepCount; i++)
        {
            intervals.Add(current);
            current += realStep;
        }
        intervals.Add(domain.End);
        return intervals;
    }
}
