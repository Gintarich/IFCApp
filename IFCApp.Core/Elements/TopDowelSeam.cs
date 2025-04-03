using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class TopDowelSeam
    {
        public List<Point3d> Points { get; set; } = new List<Point3d>();
        public double Anchorage { get; set; } = 430;
        public double FreeLength { get; set; } = 505;

        public TopDowelSeam() { }
    }
}
