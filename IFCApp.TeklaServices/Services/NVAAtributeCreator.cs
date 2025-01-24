using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //Material logic
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
    private IEnumerable<ModelObject> GetElements()
    {
        Model model = new Model();
        var mos = model.GetModelObjectSelector();
        var beams = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM).ToList();
        var plates = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE).ToList();
        var assemblies = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY).ToList();
        var combo = beams.Concat(plates).Concat(assemblies);
        return combo;
    }
    public string CreateClassificationForAllParts()
    {
        var elements = GetElements();
        List<string> errList = new List<string>();
        foreach (var element in elements)
        {
            if(element is Assembly ass)
            {
                var name = ass.Name;
                if(AttributeMapper.Clasification.TryGetValue(name, out var classification))
                {
                    ass.SetUserProperty("KLASIFIKACIJA", classification);
                    ass.Modify();
                }
                else
                {
                    if(!errList.Contains(name)) errList.Add(name);
                }
            }
            else if (element is Part part)
            {
                var name = part.Name;
                if(AttributeMapper.Clasification.TryGetValue(name, out var classification))
                {
                    part.SetUserProperty("KLASIFIKACIJA", classification);
                    part.Modify();
                }
                else
                {
                    if(!errList.Contains(name)) errList.Add(name);
                }
            }
            else
            {
                throw new NotSupportedException($"This type is not supported {element.GetType()}");
            }
        }
        StringBuilder sb = new StringBuilder();
        foreach(var err in errList)
        {
            if (err == errList[0]) sb.Append(err);
            else sb.Append(", ").Append(err);
        }
        return sb.ToString();
    }
    public void CreateAttributesForAllParts()
    {
        Model model = new Model();
        var mos = model.GetModelObjectSelector();
        var beams = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM).ToList();
        var plates = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE).ToList();
        var assemblies = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY).ToList();
        var combo = beams.Concat(plates).Concat(assemblies);
        foreach (var part in combo)
        {
            CreateAttributes(part);
        }
        model.CommitChanges();
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
        {"APDARES ĶIEĢELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"APDARES SLĀNIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"SILTUMIZOLĀCIJA", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"SIENAS PANELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"PĀRSEGUMA PANELIS", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"SMALKGRAUDAINS BETONS", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"METĀLA SIJA", "BE_07_21_05_00_Tērauda sijas"},
        {"JUMTA SIJA", "BE_07_21_05_00_Tērauda sijas"},
        {"KOLONNA", "BE_07_13_05_00_Tērauda kolonnas"},
        {"HORIZONTĀLĀ SAITE","BE_07_27_05_00_Tērauda saites"},
        {"RVL100", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"SCHOCK DORN SLD 50", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"PAMATU PLĀTNE", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"KAROGU MASTU PAMATS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"PADZIĻINĀJUMS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"PAMATA STABS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"PAMATA PĒDA", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"PAMATA SIENA", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"STABVEIDA PAMATS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"LENTVEIDA PAMATS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"MŪRA SIENA", "BE_07_15_09_00_Mūra sienas"},
        {"GRĪDA", "BE_09_01_01_00_Grīdas uz grunts"},
        {"KUBS",""},
        {"PIELI", ""},
    };
}
