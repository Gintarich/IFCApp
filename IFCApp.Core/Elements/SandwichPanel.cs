using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class SandwichPanel : Wall
    {
        public Layers Layers { get; set; }
        public int LayerCount { get; set; }
        public SandwichPanel(BBox box, Layers layers) : base(box)
        {
            Layers = layers;
        }
        public SandwichPanel(BBox box, Matrix4d cs, Layers layers) : base(box, cs)
        {
            Layers = layers;
        }
    }
}
