using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry;

public struct Point3d(double x, double y, double z) 
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Z { get; set; } = z;

    public Point3d() : this(0.0, 0.0, 0.0) { }

    public static Point3d operator+ (Point3d lhs,Point3d rhs)
    {
        return new Point3d(lhs.X+rhs.X,lhs.Y+rhs.Y,lhs.Z+rhs.Z);
    }
    public static Point3d operator- (Point3d lhs,Point3d rhs)
    {
        return new Point3d(lhs.X-rhs.X,lhs.Y-rhs.Y,lhs.Z-rhs.Z);
    }

}
