using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Window : Opening
    {
        public Window()
        {
            
        }
        public Window(BBox box, string fatherID = "") : base(box, fatherID) { }
        public Window(BBox box, Guid openingID) : base(box, new Guid(), openingID) { }
        public Window(Point3d startPoint, Point3d endPoint, string fatherID = "") : base(startPoint, endPoint, fatherID) { }
    }
}
