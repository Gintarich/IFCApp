using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Common.Collections;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.TopologyResource;

namespace IFCApp.IFCServices.Services
{
    public class IfcGeometryService
    {
        public List<Point3d> GetPoints(IfcProductRepresentation representation)
        {
            var points = new List<Point3d>();
            foreach (var repr in representation.Representations)
            {
                foreach (var item in repr.Items)
                {
                    if (item is IfcFacetedBrep faceBrep)
                    {
                        GetPoints(faceBrep, points);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            return points;
        }

        private void GetPoints(IfcFacetedBrep faceBrep, List<Point3d> points)
        {

            // Get the closed shell
            var closedShell = faceBrep.Outer;

            if (closedShell == null)
                throw new InvalidOperationException("No closed shell found in IfcFacetedBrep.");

            // Iterate over the faces in the shell
            foreach (var face in closedShell.CfsFaces)
            {
                // Process each bound in the face
                foreach (var bound in face.Bounds)
                {
                    var faceOuterBound = bound as IfcFaceOuterBound; // Or IIfcFaceBound
                    if (faceOuterBound == null) continue;

                    // Process the edges in the bound
                    var polyLoop = faceOuterBound.Bound as IfcPolyLoop;
                    if (polyLoop != null)
                    {
                        foreach (var point in polyLoop.Polygon)
                        {
                            // Extract coordinates from each point
                            points.Add(new Point3d(point.X, point.Y, point.Z));
                        }
                    }
                }
            }
        }
    }
}
