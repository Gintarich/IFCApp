using System;
using System.Collections.Generic;
using System.Text;
using IFCApp.Core;
using IFCApp.Core.Geometry;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Xml.BsConf;

namespace IFCApp.IFCServices.Utils
{
    public class TransformationService
    {
        public static readonly Matrix4d INVERSE = new Matrix4d( new double[,]
        {
            {0.515038078489869 ,   0.857167298551142,    0, -463783847.754614 },
            {-0.857167298551142,   0.515038078489869,    0, 159621882.973248 },
            {0,                    0,                    1, -32200    },
            {0,                    0,                    0, 1        }
        });

        public Matrix4d GetTransformation(IIfcObjectPlacement placement)
        {
            var cumulativeMatrix = new Matrix4d();

            // Traverse up the placement hierarchy
            while (placement != null)
            {
                // Get the current placement matrix
                var placementMatrix = GetMatrix(placement);

                // Combine with the cumulative transformation
                cumulativeMatrix = placementMatrix.Combine(cumulativeMatrix);

                // Move to the parent placement (if any)
                placement = (placement as IIfcLocalPlacement)?.PlacementRelTo;
            }
            return INVERSE.Combine(cumulativeMatrix);
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

            yAxis = zAxis.Cross(xAxis);

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

            var transformationMatrix = new Matrix4d(transformation);

            return transformationMatrix;
        }

        //Calculate inverse
        //var rot = Matrix4d.RotationZ(59);
        //var trans = Matrix4d.Translation(375689000, 315329000, 32200);
        //var tot = trans.Combine(rot);
        //var TestInv = tot.Inverse();
    }
}
