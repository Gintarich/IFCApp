using System;
using System.Collections.Generic;
using System.Text;
using IFCApp.Core;
using IFCApp.Core.Geometry;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc4.Interfaces;

namespace IFCApp.IFCServices.Utils
{
    public class TransformationService
    {
        public Matrix4d GetTransformation()
        {
            return new Matrix4d();
        }
        public Matrix4d GetMatrix(IIfcObjectPlacement placement)
        {
            if (placement is not IfcLocalPlacement localPlacement)
            {
                return new Matrix4d();
            }
            if (localPlacement.RelativePlacement is not IIfcAxis2Placement3D relativePlacement) 
            {
                return new Matrix4d();
            }
            
            var location = relativePlacement.Location;

            return new Matrix4d();
        }
    }
}
