using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Layers
    {
        public int InnerLayerThickness { get; set; }
        public int OuterLayerThickness { get; set; }
        public int InsulationThickness { get; set; }
        public Layers() { }
        public Layers(int innerLayerThickness = 0, int outerLayerThickness = 0, int izolationThickness = 0)
        {
            InnerLayerThickness = innerLayerThickness;
            OuterLayerThickness = outerLayerThickness;
            InsulationThickness = izolationThickness;
        }
    }
}
