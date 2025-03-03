using IFCApp.TeklaServices.Utils;
using IFCApp.Core.Elements;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using Tekla.Structures;
using TS = Tekla.Structures.Model;
using Tekla.Structures.Model;
using Component = Tekla.Structures.Model.Component;
using System;
using Trimble.Remoting.Collections;

namespace IFCApp.TeklaServices.Services;

public class TeklaHvacOpeningMaker
{
    private TS.Component _openingComponent;
    private string _userCreatedPropertyName = "HvacUserCreated";
    public List<Wall> Walls { get; }
    public TS.Model Model { get; }

    public TeklaHvacOpeningMaker(List<Wall> walls)
    {
        Walls = walls;
        Model = new TS.Model();
    }

    public void GenerateOpenings()
    {
        Clear();
        Model.CommitChanges();
        foreach (Wall wall in Walls)
        {
            List<HvacOpening> hvacOpenings = wall.GetHvacOpenings();
            foreach (var hvacOpening in hvacOpenings)
            {
                CutOutHvacOpening(hvacOpening);
            }
        }
        Model.CommitChanges();
    }

    private void CutOutHvacOpening(Opening opening)
    {
        if (opening is not HvacOpening hvacOpening) return;
        if (hvacOpening.IsCircle)
        {
            CutCylinderOpening(opening);
        }
        else
        {
            CutRectangleOpening(opening);
        }
    }

    private void CutRectangleOpening(Opening opening)
    {
        Point startPoint = opening.GetMaxPoint().TeklaPoint();
        Point endPoint = opening.GetMinPoint().TeklaPoint();
        Identifier identifier = new Identifier(opening.FatherID);
        var father = Model.SelectModelObject(identifier) as TS.Beam;
        if (father is null) return;
        var assembly = father.GetAssembly();
        var children = assembly.GetSecondaries().ToList<Part>();
        var parts = children.Where(x => x is Part).Cast<Part>().ToList();
        parts.Add(father);
        if (opening is not HvacOpening hvacOpening) return;
        var pts = hvacOpening.GetRect();
        foreach (var part in parts)
        {
            BooleanPart bp = new BooleanPart();
            var cp = new ContourPlate();
            pts.ForEach(x => cp.AddContourPoint(new ContourPoint(x.TeklaPoint(), new Chamfer())));
            var thickness = hvacOpening.Thickness().ToString();
            bp.Father = part;
            cp.Name = "HvacOpening";
            cp.Profile.ProfileString = thickness;
            cp.Class = BooleanPart.BooleanOperativeClassName;
            cp.Insert();
            cp.SetUserProperty("Inserted", 1);
            bp.SetOperativePart(cp);
            bp.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
            bp.Insert();
            bp.OperativePart.SetUserProperty("Inserted", 1);
            cp.Delete();
        }
    }

    private void CutCylinderOpening(Opening opening)
    {
        if (opening is not HvacOpening hvacOpening) return;
        Point startPoint = hvacOpening.GetStartPoint().TeklaPoint();
        Point endPoint = hvacOpening.GetEndPoint().TeklaPoint();
        Identifier identifier = new Identifier(opening.FatherID);
        var father = Model.SelectModelObject(identifier) as TS.Beam;
        if (father is null) return;
        var assembly = father.GetAssembly();
        var children = assembly.GetSecondaries().ToList<Part>();
        var parts = children.Where(x => x is Part).Cast<Part>().ToList();
        parts.Add(father);
        foreach (var part in parts)
        {
            BooleanPart bp = new BooleanPart();
            TS.Beam beam = new TS.Beam();
            beam.StartPoint = startPoint;
            beam.EndPoint = endPoint;
            beam.Name = "HvacOpening";
            beam.Position.Depth = Position.DepthEnum.MIDDLE;
            beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            var diameter = hvacOpening.GetDiameter().ToString();
            bp.Father = part;
            beam.Profile.ProfileString = $"D{diameter}";
            beam.Class = BooleanPart.BooleanOperativeClassName;
            beam.Insert();
            beam.SetUserProperty(_userCreatedPropertyName, 1);
            bp.SetOperativePart(beam);
            bp.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
            bp.Insert();
            bp.OperativePart.SetUserProperty(_userCreatedPropertyName, 1);
            beam.Delete();
        }
    }

    private void InsertWindow(Opening opening)
    {
        Point startPoint = opening.GetMaxPoint().TeklaPoint();
        Point endPoint = opening.GetMinPoint().TeklaPoint();
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
        var openings = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BOOLEANPART)
            .ToList().Cast<BooleanPart>();
        var openingsToDelete = openings.Where(x => IsUserCreated(x.OperativePart));
        foreach (var opening in openingsToDelete)
        {
            opening.Delete();
        }
    }
    private bool IsUserCreated(Part part)
    {
        int created = int.MaxValue;
        part.GetUserProperty(_userCreatedPropertyName, ref created);
        if (created == 1) return true;
        else return false;
    }
    public void ClearAllOpenings()
    {
        Clear();
    }
}
