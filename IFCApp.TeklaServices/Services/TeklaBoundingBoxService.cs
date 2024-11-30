using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Model;
using TS = Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services;

public class TeklaBoundingBoxService
{
    public BBox GetBox(TS.Beam beam)
    {
        var startPoint = beam.StartPoint;
        var endPoint = beam.EndPoint;
        Vector3d xAxis = new Vector3d(
            endPoint.X - startPoint.X,
            endPoint.Y - startPoint.Y,
            endPoint.Z - startPoint.Z
            ).Normalize();
        // TODO: Add check if xAxis is vertical aka xAxis == zAxis
        Vector3d yAxis = Vector3d.ZAxis.Cross(xAxis);
        Vector3d zAxis = xAxis.Cross(yAxis);
        Point3d origin = startPoint.CorePoint();
        var cs = new Matrix4d(xAxis, yAxis, zAxis, origin);
        var inverse = cs.Inverse();
        var solid = beam.GetSolid();
        var min = inverse.Apply(solid.MinimumPoint.CorePoint());
        var max = inverse.Apply(solid.MaximumPoint.CorePoint());
        var box = new BBox([min,max], cs);
        return box;
    }
}
