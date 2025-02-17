using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services
{
    public class TeklaDowelService
    {
        public void GenerateDowels(List<double> locations, Guid id1, Guid id2)
        {
            var model = new Model();
            var mos = model.GetModelObjectSelector();
            var objs = model.FetchModelObjects([new Identifier(id1), new Identifier(id2)], false);
            var slab = objs[0] as ContourPlate;
            var wall = objs[1] as Beam;

            var cs = wall.GetCoordinateSystem();

            List<Point> points = GeneratePoints(locations, cs);

            TeklaGraphicsDrawerService tgdService = new TeklaGraphicsDrawerService();
            foreach (var point in points)
            {
                tgdService.DrawCube(point);
            }
        }

        private List<Point> GeneratePoints(List<double> locations, CoordinateSystem cs)
        {
            var mat = MatrixFactory.FromCoordinateSystem(cs);
            var pts = new List<Point>();
            foreach (var location in locations)
            {
                var pt = new Point(location,0,0);
                var tformPt = mat.Transform(pt);
                pts.Add(tformPt);
            }
            return pts;
        }
    }
}
