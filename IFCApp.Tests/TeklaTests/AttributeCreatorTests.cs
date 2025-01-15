using IFCApp.TeklaServices.Services;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
            //if(part.Name == "RVL100" )
            //{
            //    Console.WriteLine("RVL");
            //}
            creator.CreateAttributes(part);
        }
    }
    [TestMethod]
    public void ShouldCreateClasification()
    {
        List<string> names = new List<string>();
        NVAAtributeCreator creator = new NVAAtributeCreator();
        Model model = new Model();
        var mos = model.GetModelObjectSelector();
        var beams = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM).ToList();
        var plates = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE).ToList();
        var assemblies = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY).ToList().Cast<Assembly>();
        var parts = beams.Concat(plates).Cast<Part>();

        foreach (var part in parts)
        {
            if (part.Name == "PIELI" && part.Class == "58")
            {
                part.Name = "APDARES ĶIEĢELIS";
                part.Modify();
            }
            if (!names.Contains(part.Name)) names.Add(part.Name);
            creator.CreateClassification(part);
        }
        foreach (var ass in assemblies)
        {
            if (!names.Contains(ass.Name)) names.Add(ass.Name);
            creator.CreateClassification(ass);
        }
        model.CommitChanges();
        File.WriteAllLines("names.txt", names);
    }
}
