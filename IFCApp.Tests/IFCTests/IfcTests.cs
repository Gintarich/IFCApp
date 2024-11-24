using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFCApp.Core.Geometry;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Utils;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;

namespace IFCApp.Tests.IFCTests;
[TestClass]
public class IfcTests
{
    [TestMethod]
    public void MustGetWindows()
    {
        IfcTester tester = new IfcTester();
        var _model = tester.GetModel();
        var kubs = _model.Instances.OfType<IfcWall>().Where(x => x.EntityLabel == 41445).ToList().FirstOrDefault();
        var tforms = new TransformationService();
        var matrix = tforms.GetMatrix(kubs.ObjectPlacement);

        var rot = Matrix4d.RotationZ(59);
        var trans = Matrix4d.Translation(375689000, 315329000, 32200);
        var tot = trans.Combine(rot);
        var inverse = tot.Inverse();

        var endMatrix = inverse.Combine(matrix);

    }
}
