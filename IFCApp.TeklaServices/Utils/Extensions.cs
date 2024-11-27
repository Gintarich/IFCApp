using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Utils;

public static class Extensions
{
        //public static List<Drawing> ToList(this DrawingEnumerator moe)
        //{
        //    var list = new List<Drawing>();
        //    while (moe.MoveNext())
        //    {
        //        list.Add(moe.Current);
        //    }
        //    return list;
        //}
        public static List<ModelObject> ToList(this ModelObjectEnumerator moe)
        {
            var list = new List<ModelObject>();
            while (moe.MoveNext())
            {
                list.Add(moe.Current);
            }
            return list;
        }
        public static List<T> ToList<T>(this IEnumerable enumerable)
        {
            var list = new List<T>();
            foreach (var item in enumerable)
            {
                T element = (T)item;
                if (element == null) continue;
                list.Add(element);
            }
            return list;
        }
        public static OBB GetObb(this Beam beam)
        {
            var cs = beam.GetCoordinateSystem();
            var matrix = MatrixFactory.ToCoordinateSystem(cs);
            var xAxis = cs.AxisX;
            var yAxis = cs.AxisY;
            var zAxis = xAxis.Cross(yAxis);
            var maxpt = beam.GetSolid().MaximumPoint;
            var minpt = beam.GetSolid().MinimumPoint;
            var midPoint = new Point((maxpt.X + minpt.X) / 2, (maxpt.Y + minpt.Y) / 2, (maxpt.Z + minpt.Z) / 2);
            var max = matrix.Transform(maxpt);
            var min = matrix.Transform(minpt);
            var length = max.X - min.X;
            var height = max.Y - min.Y;
            var width = max.Z - min.Z;
            var obb = new OBB(midPoint,xAxis,yAxis,zAxis,length,height,width);
            return obb;
        }
}
