using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Geometry3d;

namespace IFCApp.TeklaServices.Utils
{
    public static class PointExtensions
    {
        public static Point TeklaPoint(this Point3d point)
        {
            return new Point(point.X, point.Y, point.Z); 
        }

        public static Point3d CorePoint(this Point point)
        {
            return new Point3d(point.X,point.Y,point.Z);
        }
    }
}
