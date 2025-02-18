using IFCApp.IFCServices.Utils;
using System;
using System.IO;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace IFCApp.IFCServices
{
    public class IFCModel
    {

        readonly string _fileName;
        readonly IfcStore _model;

        public IFCModel(string filePath )
        {
            _model = IfcStore.Open(filePath);
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
