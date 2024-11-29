using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        var model = new IFCModel();
        _model = model;
    }
    [TestMethod]
    public void MustGetWindow()
    {
        var model = _model.GetModel();
        var labels = new List<int>() { 28820 };
        var window = model.Instances.OfType<IfcWindow>().Where(x => labels.Contains(x.EntityLabel)).ToList().FirstOrDefault();
        var openingElement = window.FillsVoids.First().RelatingOpeningElement;

        var tforms = new TransformationService();
        var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);
        BBoxService bBoxService = new BBoxService();
        var box = bBoxService.GetBBox(openingElement.Representation, matrix);
        TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
        gd.DrawBox(box);
    }
    [TestMethod]
    public void MustGetAllWindows()
    {
        var model = _model.GetModel();
        var labels = new List<int>() { 24175, 24184, 24215 };
        var windows = model.Instances.OfType<IfcWindow>();

        foreach ( var window in windows)
        {
            var openingElement = window.FillsVoids.First().RelatingOpeningElement;
            Debug.WriteLine(openingElement.Representation.Name.ToString());
            var tforms = new TransformationService();
            var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);
            BBoxService bBoxService = new BBoxService();
            var box = bBoxService.GetBBox(openingElement.Representation, matrix);
            TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
            gd.DrawBox(box);
        }
    }
}
