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

        const string fileName = "KUL-7AM-00-00-M3-AR-0001.ifc";
        readonly IfcStore _model;

        public IFCModel()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desktop, fileName );
            _model = IfcStore.Open(path);
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
