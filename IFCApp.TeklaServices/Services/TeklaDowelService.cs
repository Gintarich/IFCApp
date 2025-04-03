using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using IFCApp.Core;
using IFCApp.Core.Elements;
using Model = Tekla.Structures.Model.Model;
using Beam = Tekla.Structures.Model.Beam;
using System.Net.Http.Headers;
using Component = Tekla.Structures.Model.Component;
using Tekla.Structures.Model.Collaboration;

namespace IFCApp.TeklaServices.Services
{
    public class TeklaDowelService
    {
        public double BotWidth { get; set; }
        public double Height { get; set; }
        public double TopWidth { get; set; }
        public string InsertName { get; set; } = "InsertedBox";

        public TeklaDowelService(double botWidth = 70, double height = 300, double topWidth = 100)
        {
            BotWidth = botWidth;
            Height = height;
            TopWidth = topWidth;
        }
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
                var hLen = 35;
                var height = 280;
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
        public void GemerateDowels(Core.Model model)
        {
            var teklaModel = new Model();
            var walls = model.Elements.Where(x => x is Wall).Cast<Wall>();
            var slabs = model.Elements.Where(x => x is Slab).Cast<Slab>();
            foreach (var wall in walls)
            {
                GenerateBoxes(wall);
                GenerateAnchors(wall);
                teklaModel.CommitChanges();
            }
            foreach (var slab in slabs)
            {
                GenerateAnchors(slab);
                teklaModel.CommitChanges();
            }
        }

        private void GenerateAnchors(Slab slab)
        {
            if(slab.Anchors is null) return;
            var pts = slab.Anchors.Points;
            var anchorage = slab.Anchors.Anchorage;
            var freeLen = slab.Anchors.FreeLength;
            var id = slab.ID;
            var teklaModel = new Model();
            var part = teklaModel.SelectModelObject(new Identifier(id));
            foreach (var pt in pts)
            {
                //var globalPt = slab.Box.CS.Apply(pt);
                var globalPt = pt;
                var p1 = globalPt;
                p1.Z = p1.Z - anchorage;
                var p2 = globalPt;
                p2.Z = p2.Z + freeLen;
                SingleRebar singleRebar = new SingleRebar();
                var poly = new Polygon();
                poly.Points.Add(p1.TeklaPoint());
                poly.Points.Add(p2.TeklaPoint());
                singleRebar.Polygon = poly;
                singleRebar.Class = 8;
                singleRebar.Father = part;
                singleRebar.Grade = "B500B";
                singleRebar.Size = "16";
                singleRebar.Name = "ENKURSTIEGRA";
                singleRebar.NumberingSeries = new NumberingSeries("ENK", 1);
                singleRebar.Insert();
                singleRebar.SetUserProperty(InsertName, 1);
            }
        }

        private void GenerateAnchors(Wall wall)
        {
            TeklaGraphicsDrawerService tgdService = new TeklaGraphicsDrawerService();
            if (wall == null) return;
            if (wall.TopSeam == null) return;
            var pts = wall.TopSeam.Points;
            var anchorage = wall.TopSeam.Anchorage;
            var freeLen = wall.TopSeam.FreeLength;
            var id = wall.ID;
            var teklaModel = new Model();
            var part = teklaModel.SelectModelObject(new Identifier(id));
            foreach (var pt in pts)
            {
                var globalPt = wall.Box.CS.Apply(pt);
                var p1 = globalPt;
                p1.Z = p1.Z - anchorage;
                var p2 = globalPt;
                p2.Z = p2.Z + freeLen;
                SingleRebar singleRebar = new SingleRebar();
                var poly = new Polygon();
                poly.Points.Add(p1.TeklaPoint());
                poly.Points.Add(p2.TeklaPoint());
                singleRebar.Polygon = poly;
                singleRebar.Class = 8;
                singleRebar.Father = part;
                singleRebar.Grade = "B500B";
                singleRebar.Size = "16";
                singleRebar.Name = "ENKURSTIEGRA";
                singleRebar.NumberingSeries = new NumberingSeries("ENK", 1);
                singleRebar.Insert();
                singleRebar.SetUserProperty(InsertName, 1);
            }
        }

        private void GenerateBoxes(Wall wall)
        {
            TeklaGraphicsDrawerService tgdService = new TeklaGraphicsDrawerService();
            if (wall == null) return;
            if (wall.BottomSeam == null) return;
            var pts = wall.BottomSeam.Points;
            var dif = (TopWidth - BotWidth) / 2;
            var min = wall.Box.Min;
            var max = wall.Box.Max;
            var id = wall.ID;
            var teklaModel = new Model();
            var part = teklaModel.SelectModelObject(new Identifier(id));
            int skipBoxes = 0;
            part.GetUserProperty("SkipBoxes", ref skipBoxes);

            if (skipBoxes == 1) return;

            foreach (var pt in pts)
            {
                var points = new List<Point>();
                var halfWidth = BotWidth / 2;
                var height = Height;
                var p1 = new Point3d(pt.X + halfWidth, min.Y, min.Z);
                var p2 = new Point3d(pt.X - halfWidth, min.Y, min.Z);
                var p3 = new Point3d(pt.X - halfWidth - dif, min.Y, min.Z + Height);
                var p4 = new Point3d(pt.X + halfWidth + dif, min.Y, min.Z + Height);
                points.Add(wall.Box.CS.Apply(p1).TeklaPoint());
                points.Add(wall.Box.CS.Apply(p2).TeklaPoint());
                points.Add(wall.Box.CS.Apply(p3).TeklaPoint());
                points.Add(wall.Box.CS.Apply(p4).TeklaPoint());
                var contour = new Contour();
                foreach (var point in points)
                {
                    contour.AddContourPoint(new ContourPoint(point, new Chamfer()));
                }

                BooleanPart bp = new BooleanPart();
                bp.Father = part;
                var cp = new ContourPlate();
                cp.Contour = contour;
                cp.Name = "CutOffBox";
                cp.Profile.ProfileString = "120";
                cp.Position.Depth = Position.DepthEnum.FRONT;
                cp.Class = BooleanPart.BooleanOperativeClassName;
                cp.Insert();
                cp.SetUserProperty(InsertName, 1);
                bp.SetOperativePart(cp);
                bp.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
                bp.Insert();
                bp.OperativePart.SetUserProperty(InsertName, 1);
                cp.Delete();
            }
        }

        public void Clear()
        {
            var teklaModel = new Model();
            var selector = teklaModel.GetModelObjectSelector();
            var openings = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BOOLEANPART)
                .ToList().Cast<BooleanPart>().ToList();
            var openingsToDelete = openings.Where(x => IsUserCreated(x.OperativePart));
            var rebars = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.SINGLEREBAR);

            foreach (var opening in openingsToDelete)
            {
                opening.Delete();
            }
            foreach (var bar in rebars)
            {
                if (!(bar is SingleRebar sBar)) continue;
                if (IsUserCreated(sBar))
                { sBar.Delete(); }
            }
            teklaModel.CommitChanges();
        }

        public bool IsUserCreated(Part part)
        {
            int created = int.MaxValue;
            part.GetUserProperty(InsertName, ref created);
            if (created == 1) return true;
            else return false;
        }
        public bool IsUserCreated(SingleRebar bar)
        {
            int created = int.MaxValue;
            bar.GetUserProperty(InsertName, ref created);
            return created == 1;
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
