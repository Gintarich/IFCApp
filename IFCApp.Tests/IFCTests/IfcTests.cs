using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFCApp.Core.Geometry;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
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
    public void MustGetWindows()
    {
        var model = _model.GetModel();
        var labels = new List<int>() {24175, 24184, 24215};
        var window = model.Instances.OfType<IfcWindow>().Where(x => labels.Contains( x.EntityLabel )).ToList().FirstOrDefault();
        var openingElement = window.FillsVoids.First().RelatingOpeningElement;

        var tforms = new TransformationService();
        var openingMatrix = tforms.GetMatrix(openingElement.ObjectPlacement);
        var openingPlacement = (IfcLocalPlacement)openingElement.ObjectPlacement;
        var wallPlacement = openingPlacement.PlacementRelTo;
        var wallMatrix = tforms.GetMatrix(wallPlacement);
        var combined = wallMatrix.Combine(openingMatrix);

        var matrix = tforms.GetTransformation(openingElement.ObjectPlacement);


        var rot = Matrix4d.RotationZ(59);
        var trans = Matrix4d.Translation(375689000, 315329000, 32200);
        var tot = trans.Combine(rot);
        var inverse = tot.Inverse();

        var endMatrix = matrix.Combine(inverse);
        var wallInverse = combined.Combine(inverse);

        BBoxService bBoxService = new BBoxService(model);
        var pts = bBoxService.GetPoints(openingElement.Representation);
        var tPoints = pts.Select(x=>endMatrix.Apply(x)).ToList();
    }
}
