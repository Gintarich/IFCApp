using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IFCApp.Core.Services;

public class Colider
{
    public bool Colides(Wall wall, Opening opening)
    {
        var wallBox = wall.GetBox();
        var openingBox = opening.GetBox();
        return Colides(wallBox, openingBox);
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
