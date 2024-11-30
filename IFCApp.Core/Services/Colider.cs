using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IFCApp.Core.Services;

public class Colider
{
    public bool Colides(Wall wall, Window window)
    {
        var wallBox = wall.GetBox();
        var windowBox = window.GetBox();
        return Colides(wallBox, windowBox);
    }
    public bool Colides( BBox box, BBox other)
    {
        var thisX = box.CS.XAxis.Normalize();
        var otherX = other.CS.XAxis.Normalize();

        var isInSamePlane = thisX.Dot(otherX) > 0.99;

        if( box.Intersects(other))
        {
            return true;
        }
        return false;
    }
}
