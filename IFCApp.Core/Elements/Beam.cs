using IFCApp.Core.Geometry;
using System;

namespace IFCApp.Core.Elements;

public class Beam :ElementBase
{
    public Point3d StartPoint { get; set; }
    public Point3d EndPoint { get; set; }
    public Beam()
    {
    }
}
