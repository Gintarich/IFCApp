using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Layers(double innerLayerThickness = 0, double outerLayerThickness = 0, double izolationThickness = 0)
    {
        public double InnerLayerThickness { get; set; } = innerLayerThickness;
        public double OuterLayerThickness { get; set; } = outerLayerThickness;
        public double InsulationThickness { get; set; } = izolationThickness;
    }
}
