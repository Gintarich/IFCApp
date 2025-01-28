using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;

namespace IFCApp.TeklaServices.Utils
{
    public class TeklaInteraction
    {
        public Point3d PickPoint()
        {
            Point3d point;
            try
            {
                Picker picker = new Picker();
                Point pt = picker.PickPoint("Pick a point");
                point = new Point3d(Math.Round(pt.X), Math.Round(pt.Y), Math.Round(pt.Z));
            }
            catch
            {
                return new Point3d(0,0,0);
            }
            return point;
        }
    }
}
