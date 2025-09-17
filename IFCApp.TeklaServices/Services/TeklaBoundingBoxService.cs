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
        var dotProduct = xAxis.Dot(Vector3d.ZAxis);
        if (dotProduct < 0.99 && dotProduct > -0.99)
        {
            Vector3d yAxis = Vector3d.ZAxis.Cross(xAxis);
            Vector3d zAxis = xAxis.Cross(yAxis);
            Point3d origin = startPoint.CorePoint();
            var cs = new Matrix4d(xAxis, yAxis, zAxis, origin);
            var inverse = cs.Inverse();
            var solid = beam.GetSolid();
            var min = inverse.Apply(solid.MinimumPoint.CorePoint());
            var max = inverse.Apply(solid.MaximumPoint.CorePoint());
            var box = new BBox([min, max], cs);
            return box;
        }
        else
        {
            throw new NotImplementedException("Beam is vertical, need to implement different logic.");
        }
    }
    public BBox GetBox(TS.ContourPlate cp)
    {
        var cpt1 = cp.Contour.ContourPoints[0] as ContourPoint;
        var cpt2 = cp.Contour.ContourPoints[1] as ContourPoint;
        var p1 = cpt1.CorePoint();
        var p2 = cpt2.CorePoint();
        Vector3d xAxis = new Vector3d(
            p2.X - p1.X,
            p2.Y - p1.Y,
            p2.Z - p1.Z
            ).Normalize();
        // TODO: Add check if xAxis is vertical aka xAxis == zAxis
        Vector3d yAxis = Vector3d.ZAxis.Cross(xAxis);
        Vector3d zAxis = xAxis.Cross(yAxis);
        Point3d origin = p1;
        var cs = new Matrix4d(xAxis, yAxis, zAxis, origin);
        var inverse = cs.Inverse();
        var solid = cp.GetSolid();
        var min = inverse.Apply(solid.MinimumPoint.CorePoint());
        var max = inverse.Apply(solid.MaximumPoint.CorePoint());
        var box = new BBox([min, max], cs);
        return box;
    }
}
