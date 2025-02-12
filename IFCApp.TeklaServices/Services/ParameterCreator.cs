using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services;

public class ParameterCreator
{
    private List<string> _propNames = ["Finish", "Name", "Profile", "Material"];
    public ParameterCreator()
    {
    }

    public List<Part> Filter(List<Part> parts, BBox boxToFilterWith)
    {
        List<Part> result = new List<Part>();
        foreach (var part in parts)
        {
            BBox box = part.GetBox();
            if (boxToFilterWith.Contains(box))
            {
                result.Add(part);
            }
        }
        return result;

    }

    public void CreateParameter(string paramName, string paramValue, string partName, BBox box = null)
    {
        var parts = GetParts(box, partName);
        if (_propNames.Contains(paramName))
        {
            AddProperties(parts, paramName, paramValue);
        }
        else
        {
            AddReportProperties(parts, paramName, paramValue);
        }
    }

    private void AddReportProperties(List<Part> parts, string paramName, string paramValue)
    {
        foreach (Part part in parts)
        {
            bool success = part.SetUserProperty(paramName, paramValue);
            if (success)
            {
                part.Modify();
            }
        }
    }

    private void AddProperties(List<Part> parts, string paramName, string paramValue)
    {
        foreach (Part part in parts)
        {
            switch (paramName)
            {
                case "Name":
                    part.Name = paramValue;
                    part.Modify();
                    break;
                case "Finish":
                    part.Finish = paramValue;
                    part.Modify();
                    break;
                case "Material":
                    part.Material.MaterialString = paramValue;
                    part.Modify();
                    break;
                case "Profile":
                    part.Profile.ProfileString = paramValue;
                    part.Modify();
                    break;
                default:
                    break;
            }
        }
    }

    private List<Part> GetParts(BBox box, string partName)
    {
        Model model = new Model();
        var selector = model.GetModelObjectSelector();
        if (box is null)
        {
            var beams = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            var cplates = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE);
            List<Part> parts = new List<Part>();
            while (beams.MoveNext())
            {
                var Current = beams.Current as Part;
                if (Current is null) continue;
                if (Current.Name == partName) parts.Add(Current);
            }
            while (cplates.MoveNext())
            {
                var Current = cplates.Current as Part;
                if (Current is null) continue;
                if (Current.Name == partName) parts.Add(Current);
            }
            return parts;
        }
        else
        {
            var objs = selector.GetObjectsByBoundingBox(box.Min.TeklaPoint(), box.Max.TeklaPoint())
                .ToList().Where(o => o is Part).Cast<Part>().Where(o => o.Name == partName).ToList();
            return objs;
        }
    }

}
