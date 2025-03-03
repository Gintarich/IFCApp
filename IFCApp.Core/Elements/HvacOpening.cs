using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class HvacOpening : Opening
    {
        public bool IsCircle { get; set; }

        public HvacOpening() { }
        public HvacOpening(BBox box, Guid FatherID, Guid OpeningID) : base(box, FatherID, OpeningID) { }
        public HvacOpening(BBox box) : base(box) { }

        public double GetDiameter()
        {
            var maxPt = Box.Max;
            var minPt = Box.Min;
            var maxX = maxPt.X > minPt.X ? maxPt.X : minPt.X;
            var minX = maxPt.X > minPt.X ? minPt.X : maxPt.X;
            return maxX - minX;
        }
        public Point3d GetStartPoint()
        {
            var maxPt = Box.Max;
            var minPt = Box.Min;
            var pt = new Point3d((maxPt.X + minPt.X) / 2, maxPt.Y, (maxPt.Z + minPt.Z) / 2);
            return Box.CS.Apply(pt);
        }
        public Point3d GetEndPoint()
        {
            var maxPt = Box.Max;
            var minPt = Box.Min;
            var pt = new Point3d((maxPt.X + minPt.X) / 2, minPt.Y, (maxPt.Z + minPt.Z) / 2);
            return Box.CS.Apply(pt);
        }
        public List<Point3d> GetRect()
        {
            var maxPt = Box.Max;
            var minPt = Box.Min;
            var p1 = new Point3d(minPt.X, (maxPt.Y + minPt.Y) / 2, minPt.Z);
            var p2 = new Point3d(minPt.X, (maxPt.Y + minPt.Y) / 2, maxPt.Z);
            var p3 = new Point3d(maxPt.X, (maxPt.Y + minPt.Y) / 2, maxPt.Z);
            var p4 = new Point3d(maxPt.X, (maxPt.Y + minPt.Y) / 2, minPt.Z);
            return new List<Point3d>
            {
                Box.CS.Apply(p1),
                Box.CS.Apply(p2),
                Box.CS.Apply(p3),
                Box.CS.Apply(p4)
            };
        }
        public double Thickness()
        {
            return Math.Abs(Box.Max.Y - Box.Min.Y);
        }
    }
}
