using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json.Serialization;

namespace IFCApp.Core.Elements
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Door), typeDiscriminator: "Door")]
    [JsonDerivedType(typeof(Window), typeDiscriminator: "Window")]
    public class Opening : ElementBase
    {
        public BBox Box { get; set; }
        public Guid FatherID { get; set; }

        #region Ctors
        public Opening()
        {
            Box = new BBox();
        }
        public Opening(Point3d startPoint, Point3d endPoint, Guid fatherID)
        {
            FatherID = fatherID;
            Box = new BBox([startPoint,endPoint]);
        }
        public Opening(Point3d startPoint, Point3d endPoint, Guid fatherID, Guid openingID)
        {
            FatherID = fatherID;
            this.ID = openingID;
            Box = new BBox([startPoint,endPoint]);
        }
        public Opening(BBox box, Guid fatherID)
        {
            FatherID = fatherID;
            Box = box;
        }
        public Opening(BBox box, Guid fatherID, Guid openingID)
        {
            FatherID = fatherID;
            this.ID = openingID;
            Box = box;
        }
        public Opening(Point3d startPoint, Point3d endPoint, string fatherID = "")
        {
            if (Guid.TryParse(fatherID,out var guid)) { FatherID = guid;}
            FatherID = new Guid();
            Box = new BBox([startPoint, endPoint]);
        }
        public Opening(BBox box, string fatherID = "")
        {
            Box = box;
            if (Guid.TryParse(fatherID,out var guid)) { FatherID = guid; }
            FatherID = new Guid();
        }
        #endregion

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
