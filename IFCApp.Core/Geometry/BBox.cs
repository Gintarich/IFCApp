using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry
{
    public class BBox
    {
        public Point3d Min { get; set; }
        public Point3d Max { get; set; }
        public BBox(List<Point3d> points)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            double maxZ = double.MinValue;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                minZ = Math.Min(minZ, point.Z);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                maxZ = Math.Max(maxZ, point.Z);
            }
            Min = new Point3d(minX, minY, minZ); 
            Max = new Point3d(maxX, maxY, maxZ);
        }
    }
}
