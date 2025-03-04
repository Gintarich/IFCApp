using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services
{
    public class TeklaDowelService
    {
        public void GenerateDowels(List<Point3d> points, Guid id1, Guid id2)
        {
            var model = new Model();
            var mos = model.GetModelObjectSelector();
            var objs = model.FetchModelObjects([new Identifier(id1), new Identifier(id2)], false);
            var slab = objs[0] as ContourPlate;
            var wall = objs[1] as Beam;

            var cs = wall.GetCoordinateSystem();
            var tp = new TransformationPlane(cs);
            var toLocal = tp.TransformationMatrixToLocal;
            var toGlobal = tp.TransformationMatrixToGlobal;
            var solid = wall.GetSolid();
            var minPtLoc = toLocal.Transform(solid.MinimumPoint);
            var maxPtLoc = toLocal.Transform(solid.MaximumPoint);
            var minP = new Point(Math.Min(minPtLoc.X, maxPtLoc.X),
                Math.Min(minPtLoc.Y, maxPtLoc.Y),
                Math.Min(minPtLoc.Z, maxPtLoc.Z));
            var maxP = new Point(Math.Max(minPtLoc.X, maxPtLoc.X),
                Math.Max(minPtLoc.Y, maxPtLoc.Y),
                Math.Max(minPtLoc.Z, maxPtLoc.Z));

            List<Point> tPts = points.Select(x => x.TeklaPoint()).ToList();

            TeklaGraphicsDrawerService tgdService = new TeklaGraphicsDrawerService();
            foreach (var point in tPts)
            {
                var pts = new List<Point>();
                var hLen = 50;
                var height = 200;
                var localPt = toLocal.Transform(point);
                var p1 = new Point(localPt.X + hLen, minP.Y, maxP.Z);
                var p2 = new Point(localPt.X - hLen, minP.Y, maxP.Z);
                var p3 = new Point(localPt.X - hLen - 10, minP.Y + height, maxP.Z);
                var p4 = new Point(localPt.X + hLen + 10, minP.Y + height, maxP.Z);
                pts.Add(toGlobal.Transform(p1));
                pts.Add(toGlobal.Transform(p2));
                pts.Add(toGlobal.Transform(p3));
                pts.Add(toGlobal.Transform(p4));

                foreach (var pt in pts)
                {
                    tgdService.DrawCube(pt, 10);
                }
            }
        }

        private List<Point> GeneratePoints(List<double> locations, CoordinateSystem cs)
        {
            var mat = MatrixFactory.FromCoordinateSystem(cs);
            var pts = new List<Point>();
            foreach (var location in locations)
            {
                var pt = new Point(location, 0, 0);
                var tformPt = mat.Transform(pt);
                pts.Add(tformPt);
            }
            return pts;
        }
    }
}
