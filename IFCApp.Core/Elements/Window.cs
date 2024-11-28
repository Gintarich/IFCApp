using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Window
    {
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }
        public int FatherID { get; set; }

        public Window(Point3d startPoint, Point3d endPoint, int fatherID)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            FatherID = fatherID;
        }

        public Point3d GetEndPoint()
        {
            return EndPoint;
        }

        public Point3d GetStartPoint()
        {
            return StartPoint;
        }
    }
}
