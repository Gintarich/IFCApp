using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Window
    {
        private BBox _box;
        public int FatherID { get; set; }

        public Window(Point3d startPoint, Point3d endPoint, int fatherID = 0)
        {
            FatherID = fatherID;
            _box = new BBox([startPoint, endPoint]);
        }
        public Window(BBox box, int fatherID = 0)
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
            return _box.Min;
        }

        public Point3d GetStartPoint()
        {
            return _box.Max;
        }
    }
}
