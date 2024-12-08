using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class WallPanel : Wall
    {
        public WallPanel(BBox box) : base(box)
        {
        }

        public WallPanel(BBox box, Matrix4d cs) : base(box, cs)
        {
        }
    }
}
