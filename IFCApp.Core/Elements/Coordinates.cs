using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public class Coordinates
    {
        public Matrix4d Matrix { get; set; }
        public double North { get; set; }
        public double East { get; set; }
        public double Angle { get; set; }
        public double Elevation { get; set; }
        public Coordinates(double north, double east, double angle, double elevation, Matrix4d mat)
        {
            North = north;
            East = east;
            Angle = angle;
            Elevation = elevation;
            Matrix = mat;
        }

        public Coordinates(double north, double east, double elevation, double angle)
        {
            North = north;
            East = east;
            Angle = angle;
            Elevation = elevation;
            Matrix = new Matrix4d(east,north,elevation,angle).InverseRigid();
        }

        public Coordinates()
        {
            North = 0;
            East = 0;
            Angle = 0;
            Elevation = 0;
            Matrix = new Matrix4d();
        }

    }
}
