using IFCApp.Core.Elements;
using IFCApp.IFCServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc2x3.SharedBldgElements;

namespace IFCApp.IFCServices.Services
{
    public class IfcWindowService
    {
        private IFCModel _model;

        public IfcWindowService(IFCModel model)
        {
            _model = model;
        }

        public List<Window> GetWindows()
        {
            var windowsOut = new List<Window>();
            var model = _model.GetModel();
            var windows = model.Instances.OfType<IfcWindow>();

            foreach (var window in windows)
            {
                var openingElement = window.FillsVoids.First().RelatingOpeningElement;
                var tforms = new TransformationService();
                var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);
                BBoxService bBoxService = new BBoxService();
                var box = bBoxService.GetBBox(openingElement.Representation, matrix).TrimBoxWidth(1000);
                windowsOut.Add(new Window(box));
            }
            return windowsOut;
        }
    }
}
