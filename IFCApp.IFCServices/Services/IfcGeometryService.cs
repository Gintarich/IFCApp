using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using IFCApp.IFCServices.Utils;
using System.Text;
using Xbim.Common.Collections;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.TopologyResource;

namespace IFCApp.IFCServices.Services
{
    public class IfcGeometryService
    {
        public List<Point3d> GetPoints(IfcProductRepresentation representation)
        {
            var points = new List<Point3d>();
            List<Point3d> newPoints = new List<Point3d>();
            foreach (var repr in representation.Representations)
            {
                foreach (var item in repr.Items)
                {
                    if (item is IfcFacetedBrep faceBrep)
                    {
                        GetPoints(faceBrep, newPoints);
                    }
                    if (item is IfcExtrudedAreaSolid sweptAreaSolid)
                    {
                        GetPoints(sweptAreaSolid, points);
                        var ifcMat = sweptAreaSolid.Position.ToMatrix3D();
                        var mat = ifcMat.ToCoreMat();
                        foreach (var pt in points)
                        {
                            var tPt = mat.Apply(pt);
                            newPoints.Add(tPt);
                        }
                    }
                    else
                    {
                        //throw new NotImplementedException();
                    }
                }
            }
            return newPoints;
        }

        private void GetPoints(IfcExtrudedAreaSolid sweptAreaSolid, List<Point3d> points)
        {
            var profile = sweptAreaSolid.SweptArea as IfcProfileDef;
            if (profile is IfcRectangleProfileDef rect)
            {
                var offset = 500;
                var xDim = rect.XDim;
                var yDim = rect.YDim;
                var posX = rect.Position.Location.X;
                var posY = rect.Position.Location.Y;
                var posZ = rect.Position.Location.Z;
                var dir1 = sweptAreaSolid.ExtrudedDirection.X;
                var dir2 = sweptAreaSolid.ExtrudedDirection.Y;
                var dir3 = sweptAreaSolid.ExtrudedDirection.Z;
                var depth = sweptAreaSolid.Depth + offset;
                if (double.IsNaN(posZ)) posZ = 0 - offset/2;
                else posZ = posZ - offset/2;
                points.AddRange(GetBox(xDim, yDim, depth, new Point3d(posX, posY, posZ), new Vector3d(dir1, dir2, dir3)));
            }

            else
            {
                throw new NotImplementedException($"Geometry service doesnt have definition for {profile.GetType()}");
            }
        }

        private List<Point3d> GetBox(double xDim, double yDim, double depth, Point3d pos, Vector3d dir)
        {
            List<Point3d> boxPoints = new List<Point3d>();
            var topLeft = new Point3d(pos.X - xDim / 2, pos.Y + yDim / 2, pos.Z);
            var topRight = new Point3d(pos.X + xDim / 2, pos.Y + yDim / 2, pos.Z);
            var bottomLeft = new Point3d(pos.X - xDim / 2, pos.Y - yDim / 2, pos.Z);
            var bottomRight = new Point3d(pos.X + xDim / 2, pos.Y - yDim / 2, pos.Z);

            var eDir = dir * depth;
            var topLeftExtruded = new Point3d(topLeft.X + eDir.X, topLeft.Y + eDir.Y, topLeft.Z + eDir.Z);
            var topRightExtruded = new Point3d(topRight.X + eDir.X, topRight.Y + eDir.Y, topRight.Z + eDir.Z);
            var bottomLeftExtruded = new Point3d(bottomLeft.X + eDir.X, bottomLeft.Y + eDir.Y, bottomLeft.Z + eDir.Z);
            var bottomRightExtruded = new Point3d(bottomRight.X + eDir.X, bottomRight.Y + eDir.Y, bottomRight.Z + eDir.Z);

            boxPoints.Add(topLeft);
            boxPoints.Add(topRight);
            boxPoints.Add(bottomRight);
            boxPoints.Add(bottomLeft);
            boxPoints.Add(topLeftExtruded);
            boxPoints.Add(topRightExtruded);
            boxPoints.Add(bottomRightExtruded);
            boxPoints.Add(bottomLeftExtruded);
            return boxPoints;
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
