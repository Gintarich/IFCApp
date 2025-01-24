using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Opening : ElementBase
    {
        public BBox Box { get; set; }
        public int FatherID { get; set; }

        public Opening()
        {
            Box = new BBox();
        }
        public Opening(Point3d startPoint, Point3d endPoint, int fatherID = 0)
        {
            FatherID = fatherID;
            Box = new BBox([startPoint, endPoint]);
        }
        public Opening(BBox box, int fatherID = 0)
        {
            Box = box;
            FatherID = fatherID;
        }
        public BBox GetBox()
        {
            return Box;
        }
        public Point3d GetEndPoint()
        {
            var pt = Box.GetMin();
            pt.Round(0);
            return pt;
        }

        public Point3d GetStartPoint()
        {
            var pt = Box.GetMax();
            pt.Round(0);
            return pt;
        }
    }
}
