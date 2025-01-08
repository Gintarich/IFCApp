using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services;

public class NVAAtributeCreator
{
    public NVAAtributeCreator()
    {
    }
    public void CreateAttributes(ModelObject mo)
    {
        if (mo is null) return;
        switch (mo)
        {
            case Assembly ass:
                CreateAttributesForAssembly(ass);
                break;
            case Part part:
                CreateAttributesForPart(part);
                break;
        };
        new Model().CommitChanges();
    }

    private void CreateAttributesForPart(Part pt)
    {
        string teklaMat = pt.GetStringProp("MATERIAL_TYPE");
        if (teklaMat == "MISCELLANEOUS") teklaMat = pt.Material.MaterialString;
        var material = string.Empty;
        var name = pt.Name;
        try
        {
            material = AttributeMapper.Material[teklaMat];
        }
        catch (KeyNotFoundException e)
        {
            throw new KeyNotFoundException($"The value that broke dict {teklaMat}");
        }
        pt.SetUserProperty("NOSAUKUMS", name);
        pt.SetUserProperty("MATERIALS", material);
        pt.Modify();
    }

    private void CreateAttributesForAssembly(Assembly ass)
    {
        var name = ass.Name;
        string material = string.Empty;
        if (!AttributeMapper.MaterialFromNames.TryGetValue(name, out material))
        {
            var mp = ass.GetMainPart() as Part;
            var materialType = mp.GetStringProp("MATERIAL_TYPE");
            if (materialType == "MISCELLANEOUS")
            {
                var materialName = mp.Material.MaterialString;
                material = AttributeMapper.Material[materialName];
            }
            else
            {
                material = AttributeMapper.Material[materialType];
            }
        }
        ass.SetUserProperty("NOSAUKUMS", name);
        ass.SetUserProperty("MATERIALS", material);
        ass.Modify();
    }
}

public class AttributeMapper
{
    public static Dictionary<string, string> Material { get; set; } = new Dictionary<string, string>
    {
        { "CONCRETE", "DZELZSBETONS" },
        { "Keramzitbetons", "KERAMZĪTBETONS" },
        { "STEEL", "TĒRAUDS" },
        { "Insulation_hard", "IZOLĀCIJA" },
        { "KOOLTHERM K20", "IZOLĀCIJA" }
    };
    public static Dictionary<string, string> MaterialFromNames { get; set; } = new Dictionary<string, string>
    {
        {"SIENAS PANELIS", "DZLEZSBETONA PANELIS AR SILTUMIZOLĀCIJU" }
    };
}
