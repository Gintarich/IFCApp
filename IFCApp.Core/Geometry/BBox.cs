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
            double minX = 0;
            double minY = 0;
            double minZ = 0;
            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;

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
