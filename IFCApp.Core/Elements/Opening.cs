using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Opening
    {
        private BBox _box;
        public int FatherID { get; set; }

        public Opening(Point3d startPoint, Point3d endPoint, int fatherID = 0)
        {
            FatherID = fatherID;
            _box = new BBox([startPoint, endPoint]);
        }
        public Opening(BBox box, int fatherID = 0)
        {
            _box = box;
            FatherID = fatherID;
        }
        public BBox GetBox()
        {
            return _box;
        }
        public Point3d GetEndPoint()
        {
            var pt = _box.Min;
            pt.Round(0);
            return pt;
        }

        public Point3d GetStartPoint()
        {
            var pt = _box.Max;
            pt.Round(0);
            return pt;
        }
    }
}
