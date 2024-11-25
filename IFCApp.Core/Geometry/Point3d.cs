using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry;

public struct Point3d(double x = 0, double y = 0, double z = 0)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Z { get; set; } = z;

    public Point3d() : this(0.0, 0.0, 0.0) { }

    public void Round(int precision)
    {
        this.X = Math.Round(this.X, precision);
        this.Y = Math.Round(this.Y, precision);
        this.Z = Math.Round(this.Z, precision);
    }

    public static Point3d operator +(Point3d lhs, Point3d rhs)
    {
        return new Point3d(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
    }
    public static Point3d operator -(Point3d lhs, Point3d rhs)
    {
        return new Point3d(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
    }
    public double this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Index must be 0, 1, or 2.")
            };
        }
        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                default: throw new IndexOutOfRangeException("Index must be 0, 1, or 2.");
            }
        }
    }
}
