using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Window
    {
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }
        public int FatherID { get; set; }
    }
}
