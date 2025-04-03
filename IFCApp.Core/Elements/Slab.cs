using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Slab : ElementBase
    {
        public BBox Box { get; set; }
        public TopDowelSeam Anchors { get; set; } = new TopDowelSeam();
        public Slab() { }
        public Slab(BBox box)
        {
            Box = box;
        }
    }
}
