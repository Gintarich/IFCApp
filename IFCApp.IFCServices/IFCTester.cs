using IFCApp.IFCServices.Utils;
using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace IFCApp.IFCServices
{
    public class IfcTester
    {
        const string fileName = @"C:\Users\Admin\Desktop\KUL-7AM-00-00-M3-AR-0001.ifc";
        readonly IfcStore _model;

        public IfcTester()
        {
            _model = IfcStore.Open(fileName);
        }
        public IfcStore GetModel()
        {
            return _model;
        }
        public void GetTransformation()
        {
        }
    }
}
