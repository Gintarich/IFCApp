using IFCApp.Core.Geometry;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Tessellator;

namespace IFCApp.IFCServices.Services
{
    public class BBoxService
    {
        private IfcGeometryService _gService = new IfcGeometryService();
        public BBoxService()
        {
        }
        private List<Point3d> GetPoints(IfcProductRepresentation product)
        {
            var points = _gService.GetPoints(product);
            return points;
        }
        public BBox GetBBox(IfcProductRepresentation product, Matrix4d tForm)
        {
            var points = GetPoints(product);
            var trimmedPoints = TrimPointsY(points, 500);
            var ptsOut = trimmedPoints.Select(tForm.Apply).ToList();
            return new BBox(ptsOut);
        }
        public List<Point3d> TrimPointsY(List<Point3d> pts, double amount)
        {
            List<Point3d> ptsOut = new List<Point3d>();
            foreach (var pt in pts)
            {
                if(pt.Y>amount)
                {
                    ptsOut.Add(new Point3d(pt.X,amount,pt.Z));
                }
                else if(pt.Y < -amount)
                {
                    ptsOut.Add(new Point3d(pt.X,-amount,pt.Z));
                }
                else
                {
                    ptsOut.Add(pt);
                }
            }
            return ptsOut;
        }
    }
}
