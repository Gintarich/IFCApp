﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
using IFCApp.TeklaServices.Utils;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;

namespace IFCApp.Tests.IFCTests;
[TestClass]
public class IfcTests
{
    public IFCModel _model { get; set; }
    public IfcTests()
    {
        var model = new IFCModel("DZI-7AM-00-00-M3-AR-0001.ifc");
        _model = model;
    }
    [TestMethod]
    public void MustGetWindow()
    {
        var model = _model.GetModel();
        var labels = new List<int>() { 11358 };
        var window = model.Instances.OfType<IfcWindow>().Where(x => labels.Contains(x.EntityLabel)).ToList().FirstOrDefault();
        var openingElement = window.FillsVoids.First().RelatingOpeningElement;

        var tforms = new TransformationService(VUGDCoordinateSystems.InverseBol);
        var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);
        BBoxService bBoxService = new BBoxService();
        var box = bBoxService.GetBBox(openingElement.Representation, matrix);
        TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
        gd.DrawBox(box);
    }
    [TestMethod]
    public void MustGetAllWindows()
    {
        TransformationService transformationService = new TransformationService(VUGDCoordinateSystems.InverseDzin);
        BBoxService boxService = new BBoxService();
        IfcWindowService serv = new IfcWindowService(_model,transformationService,boxService);
        var windows = serv.GetWindows();
        foreach (var window in windows)
        {
            TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
            gd.DrawBox(window.GetBox());
        }
    }
    [TestMethod]
    public void MustGetAllDoors()
    {
        TransformationService transformationService1 = new TransformationService(VUGDCoordinateSystems.InverseDzin);
        BBoxService boxService1 = new BBoxService();
        IfcDoorService doorService = new IfcDoorService(_model,transformationService1,boxService1);
        var doors = doorService.GetDoors();
        foreach (var door in doors)
        {
            TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
            gd.DrawBox(door.GetBox());
        }
    }
}
