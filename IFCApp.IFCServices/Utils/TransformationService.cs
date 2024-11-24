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

            Vector3d xAxis = Vector3d.xAxis;
            Vector3d yAxis = Vector3d.yAxis;
            Vector3d zAxis = Vector3d.zAxis;

            if (relativePlacement.Axis != null)
            {
                zAxis = new Vector3d(
                    relativePlacement.Axis.DirectionRatios[0],
                    relativePlacement.Axis.DirectionRatios[1],
                    relativePlacement.Axis.DirectionRatios[2]
                    );
            }

            if (relativePlacement.RefDirection != null)
            {
                xAxis = new Vector3d(
                    relativePlacement.RefDirection.DirectionRatios[0],
                    relativePlacement.RefDirection.DirectionRatios[1],
                    relativePlacement.RefDirection.DirectionRatios[2]
                    );
            }
            
            yAxis = xAxis.Cross( zAxis );

            xAxis.Normalize();
            yAxis.Normalize();
            zAxis.Normalize();

            var transformation = new double[,]
            {
                {xAxis.X, yAxis.X, zAxis.X, location.X},
                {xAxis.Y, yAxis.Y, zAxis.Y, location.Y},
                {xAxis.Z, yAxis.Z, zAxis.Z, location.Z},
                {0,       0,       0,       1         }
            };

            var transformationMatrix = new Matrix4d( transformation );

            return transformationMatrix;
        }
    }
}
