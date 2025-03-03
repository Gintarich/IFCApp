using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Recess:Opening
    {
        public Recess(BBox box, Guid FatherID, Guid OpeningID) : base(box, FatherID, OpeningID) { }
    }
}
