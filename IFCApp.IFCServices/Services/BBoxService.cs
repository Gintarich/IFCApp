using IFCApp.Core.Geometry;
using System.Collections.Generic;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Tessellator;

namespace IFCApp.IFCServices.Services
{
    public class BBoxService
    {
        private IfcStore _model;
        public BBoxService(IfcStore model)
        {
            _model = model;
        }
        public List<Point3d> GetPoints(IfcProductRepresentation product)
        {
            var gService = new IfcGeometryService();
            var points = gService.GetPoints(product);
            return points;
        }
    }
}
