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
    public void CreateClassification(Assembly ass)
    {
        var name = ass.Name;
        if (AttributeMapper.Clasification.TryGetValue(name, out var clasification))
        {
            ass.SetUserProperty("KLASIFIKACIJA", clasification);
            ass.Modify();
        }
    }
    public void CreateClassification(Part part)
    {
        var name = part.Name;
        if (AttributeMapper.Clasification.TryGetValue(name, out var clasification))
        {
            part.SetUserProperty("KLASIFIKACIJA", clasification);
            part.Modify();
        }
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
    public static Dictionary<string, string> Clasification { get; set; } = new Dictionary<string, string>
    {
        {"IZOLĀCIJA", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas" },
        {"NESOŠAIS SLĀNIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"PIELI", ""},
        {"PĀRSEGUMA PANELIS", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"APDARES ĶIEĢELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"METĀLA SIJA", "BE_07_21_05_00_Tērauda sijas" },
        {"APDARES SLĀNIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"SILTUMIZOLĀCIJA", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"PADZIĻINĀJUMS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"RVL100","BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"SMALKGRAUDAINS BETONS", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"PAMATU PLĀTNE", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"KUBS",""},
        {"SIENAS PANELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"SCHOCK DORN SLD 50", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
    };
}
