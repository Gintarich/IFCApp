using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Window : Opening
    {
        public Window(BBox box, int fatherID = 0) : base(box, fatherID)
        {
        }

        public Window(Point3d startPoint, Point3d endPoint, int fatherID = 0) : base(startPoint, endPoint, fatherID)
        {
        }
    }
}
