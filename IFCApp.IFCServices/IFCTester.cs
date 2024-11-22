using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace IFCApp.IFCServices
{
    public class IfcTester
    {
        const string fileName = @"C:\Users\Admin\Desktop\KUL-7AM-00-00-M3-AR-0001.ifc";
        public void GetWalls()
        {
            IfcStore model = IfcStore.Open(fileName);
            var windows = model.Instances.OfType<IfcWindow>().ToList();
        }
    }
}
