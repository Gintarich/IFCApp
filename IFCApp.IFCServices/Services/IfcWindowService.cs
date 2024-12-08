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
        private TransformationService _transformationService;
        private BBoxService _boxService;

        public IfcWindowService(IFCModel model, TransformationService transformationService, BBoxService boxService)
        {
            _model = model;
            _transformationService = transformationService;
            _boxService = boxService;
        }

        public List<Window> GetWindows()
        {
            var windowsOut = new List<Window>();
            var model = _model.GetModel();
            var windows = model.Instances.OfType<IfcWindow>();

            foreach (var window in windows)
            {
                var openingElement = window.FillsVoids.First().RelatingOpeningElement;
                var tforms = _transformationService;
                var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);
                BBoxService bBoxService = _boxService;
                var box = bBoxService.GetBBox(openingElement.Representation, matrix).TrimBoxWidth(1000);
                windowsOut.Add(new Window(box));
            }
            return windowsOut;
        }
    }
}
