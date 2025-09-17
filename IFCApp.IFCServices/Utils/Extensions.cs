using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Common.Geometry;

namespace IFCApp.IFCServices.Utils
{
    public static class Extensions
    {
        public static Matrix4d ToCoreMat(this XbimMatrix3D mat)
        {
            return new Matrix4d(new double[,] {
                { mat.M11, mat.M21, mat.M31, mat.OffsetX },
                { mat.M12, mat.M22, mat.M32, mat.OffsetY },
                { mat.M13, mat.M23, mat.M33, mat.OffsetZ },
                { 0,0,0,1 }});
        }
    }
}
