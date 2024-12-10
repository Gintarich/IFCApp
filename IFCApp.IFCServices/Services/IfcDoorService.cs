using IFCApp.Core.Elements;
using IFCApp.IFCServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.SharedBldgElements;

namespace IFCApp.IFCServices.Services;

public class IfcDoorService
{
    private IFCModel _model;
    private TransformationService _transformationService;
    private BBoxService _boxService;

    public IfcDoorService(IFCModel model, TransformationService transformationService, BBoxService boxService)
    {
        _model = model;
        _transformationService = transformationService;
        _boxService = boxService;
    }
    public List<Door> GetDoor()
    {
        var doorsOut = new List<Door>();
        var model = _model.GetModel();
        var doors = model.Instances.OfType<IfcDoor>();

        foreach (var door in doors)
        {
            var openingElement = door.FillsVoids.First().RelatingOpeningElement;
            var tforms = _transformationService;
            var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);
            BBoxService bBoxService = _boxService;
            var box = bBoxService.GetBBox(openingElement.Representation, matrix).TrimBoxWidth(1000);
            doorsOut.Add(new Door(box));
        }
        return doorsOut;
    }
}
