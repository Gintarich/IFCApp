using IFCApp.TeklaServices.Services;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace IFCApp.Tests.TeklaTests;

[TestClass]
public class AttributeCreatorTests
{
    [TestMethod]
    public void ShouldCreateAttributes()
    {
        NVAAtributeCreator creator = new NVAAtributeCreator();
        Model model = new Model();
        var mos = model.GetModelObjectSelector();
        var beams = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM).ToList();
        var plates = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE).ToList();
        var assemblies = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY).ToList();
        var combo = beams.Concat(plates).Concat(assemblies);
        foreach ( var part in combo)
        {
            creator.CreateAttributes(part);
        }
    }
}
