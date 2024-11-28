using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc;

namespace IFCApp.IFCServices.Services
{
    public class IfcWindowService
    {
        private IfcStore _model;

        public IfcWindowService(IfcStore model)
        {
            _model = model; 
        }

        public List<Window> GetWindows()
        {
            return new List<Window>();
        }
    }
}
