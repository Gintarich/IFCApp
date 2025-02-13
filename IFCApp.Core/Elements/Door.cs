using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Door : Opening
    {
        public Door()
        {
            
        }
        public Door(BBox box, string fatherID = "") : base(box, fatherID) { }
        public Door(BBox box, Guid openingID) : base(box, new Guid(), openingID) { }
        public Door(Point3d startPoint, Point3d endPoint, string fatherID = "") : base(startPoint, endPoint, fatherID) { }
    }
}
