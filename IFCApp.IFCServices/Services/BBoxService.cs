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
            return new BBox(points, tForm);
        }
    }
}
