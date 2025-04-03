using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using System.Text;
using IFCApp.TeklaServices.Utils;
using System.Linq;

namespace IFCApp.TeklaServices.Services;

public class TeklaSlabService
{

    private TeklaBoundingBoxService _boundingBoxService;

    public TeklaSlabService(TeklaBoundingBoxService boundingBoxService)
    {
        _boundingBoxService = boundingBoxService;
    }
    public List<Slab> GetSlabs(List<string> AssemblyNames)
    {
        List<Slab> slabs = new List<Slab>();
        var model = new TSM.Model();
        var selector = model.GetModelObjectSelector();
        var allAssemblies = selector.GetAllObjectsWithType(TSM.ModelObject.ModelObjectEnum.ASSEMBLY)
            .ToList().Cast<TSM.Assembly>();
        TeklaBoundingBoxService bbService = _boundingBoxService;
        var slabAssemblies = allAssemblies.Where(x => AssemblyNames.Contains(x.Name))
           .Cast<TSM.Assembly>();
        foreach (var slab in slabAssemblies)
        {
            var mainPart = slab.GetMainPart() as TSM.ContourPlate;
            if (mainPart != null)
            {
                var slb = new Slab();
                var box = bbService.GetBox(mainPart);
                slb.Box = box;
                slb.ID = mainPart.Identifier.GUID;
                slabs.Add(slb);
            }
        }
        return slabs;
    }
}
