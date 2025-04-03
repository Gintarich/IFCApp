using IFCApp.TeklaServices.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services;

public class NVAAtributeCreator
{
    private ArrayList _stringProps = new ArrayList
    {
        "MATERIAL_TYPE",
        "ASSEMBLY_POS",
        "BOTTOM_LEVEL",
        "TOP_LEVEL",
    };
    private ArrayList _doubleProps = new ArrayList
    {
        "AREA",
        "HEIGHT",
        "WIDTH",
        "LENGTH",
        "VOLUME",
        "WEIGHT",
        "AREA_PROJECTION_XY_NET",
        "AREA_PROJECTION_XY_GROSS"
    };
    private ArrayList _intProps = new ArrayList
    {

    };
    private List<string> _walls = new List<string>
    {
        "APDARES SLĀNIS","SILTUMIZOLĀCIJA",
        "NESOŠAIS SLĀNIS", "TRĪSSLĀŅU SIENAS PANELIS",
        "VIENSLĀŅU SIENAS PANELIS", "APDARES ĶIEĢELIS"
    };
    private List<string> _slabs = new List<string>
    {
        "PĀRSEGUMA PANELIS", "PAMATU PLĀTNE",
        "KAROGU MASTU PAMATS",
    };
    private List<string> _petraParts = new List<string>
    {
        "PETRA","Sideplate1","Sideplate1st","Roundbar1","Plate1", "Plate2"
    };
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
        Hashtable table = new Hashtable();
        pt.GetAllReportProperties(_stringProps, _doubleProps, _intProps, ref table);
        var teklaMat = table["MATERIAL_TYPE"] as string;
        string prof = pt.Profile.ProfileString;


        if (teklaMat == "MISCELLANEOUS") teklaMat = pt.Material.MaterialString;
        var material = string.Empty;
        var name = pt.Name;
        if(name == "Sideplate1")
        {
            var s = 0;
        }
        try
        {
            material = AttributeMapper.Material[teklaMat];
        }
        catch (KeyNotFoundException e)
        {
            throw new KeyNotFoundException($"The value that broke dict {teklaMat}");
        }
        SetName(pt, name);
        pt.SetUserProperty("MATERIALS", material);
        AddProperty(pt, "03_TIPS", table["ASSEMBLY_POS"] as string);
        //if ((table["ASSEMBLY_POS"] as string == "TP/1003") && pt.Name == "NESOŠAIS SLĀNIS")
        //{
        //    var s = 0;
        //}
        AddProperty(pt, "04_SKERSGRIEZUMS", GetSection(pt.Profile.ProfileString).Profile);
        var height = Math.Round((double)table["HEIGHT"], 0);
        if (_walls.Contains(name)) AddProperty(pt, "05_AUGSTUMS", height);
        if (_walls.Contains(name) || _slabs.Contains(name)) AddProperty(pt, "07_BIEZUMS", Math.Round((double)table["WIDTH"], 0));
        AddProperty(pt, "08_GARUMS", Math.Round((double)table["LENGTH"], 0));
        AddProperty(pt, "09_PLATĪBA", Math.Round((double)table["AREA_PROJECTION_XY_NET"], 0));
        AddProperty(pt, "10_TILPUMS", Math.Round((double)table["VOLUME"], 0));
        AddProperty(pt, "11_SVARS", Math.Round((double)table["WEIGHT"], 2));
        AddProperty(pt, "12_ELEMENTA_AU_ATZ", (string)table["TOP_LEVEL"]);
        AddProperty(pt, "13_ELMENETA_AP_ATZ", (string)table["BOTTOM_LEVEL"]);
        AddProperty(pt, "15_TONIS", "N/A");
        AddProperty(pt, "16_KLASE", pt.Material.MaterialString);
        AddProperty(pt, "17_IEDARBIBAS_KL", pt.Finish);
        AddProperty(pt, "18_UGUNSIEDARB_KL", "N/A");
        AddProperty(pt, "19_PĀRKLĀJUMS", "N/A");
        pt.Modify();
    }

    private void CreateAttributesForAssembly(Assembly ass)
    {
        Hashtable table = new Hashtable();
        ass.GetAllReportProperties(_stringProps, _doubleProps, _intProps, ref table);
        var name = ass.Name;
        string material = string.Empty;
        var mp = ass.GetMainPart() as Part;
        if (mp is null) return;
        //Material logic
        if (!AttributeMapper.MaterialFromNames.TryGetValue(name, out material))
        {
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
        AddProperty(ass, "03_TIPS", table["ASSEMBLY_POS"] as string);
        AddProperty(ass, "04_SKERSGRIEZUMS", GetSection(mp.Profile.ProfileString).Profile);
        var height = Math.Round((double)table["HEIGHT"], 0);
        if (_walls.Contains(name)) AddProperty(ass, "05_AUGSTUMS", height);
        if (_walls.Contains(name) || _slabs.Contains(name)) AddProperty(ass, "07_BIEZUMS", Math.Round((double)table["WIDTH"], 0));
        AddProperty(ass, "08_GARUMS", Math.Round((double)table["LENGTH"], 0));
        AddProperty(ass, "09_PLATĪBA", Math.Round((double)table["AREA_PROJECTION_XY_NET"], 0));
        AddProperty(ass, "10_TILPUMS", Math.Round((double)table["VOLUME"], 0));
        AddProperty(ass, "11_SVARS", Math.Round((double)table["WEIGHT"], 2));
        AddProperty(ass, "12_ELEMENTA_AU_ATZ", (string)table["TOP_LEVEL"]);
        AddProperty(ass, "13_ELMENETA_AP_ATZ", (string)table["BOTTOM_LEVEL"]);
        AddProperty(ass, "15_TONIS", "N/A");
        AddProperty(ass, "16_KLASE", "N/A");
        AddProperty(ass, "17_IEDARBIBAS_KL", "N/A");
        AddProperty(ass, "18_UGUNSIEDARB_KL", "N/A");
        AddProperty(ass, "19_PĀRKLĀJUMS", "N/A");
        ass.Modify();
    }
    private void CreateAttributesForRebar(Reinforcement reinforcement)
    {
        int count = int.MinValue;
        reinforcement.GetReportProperty("NUMBER", ref count);
        double len = double.MinValue;
        reinforcement.GetReportProperty("LENGTH", ref len);
        if (count > 0)
        {
            var totalLen = len * count;
            reinforcement.SetUserProperty("TOTAL_LENGTH", totalLen);
        }
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
            if (element is Assembly ass)
            {
                var name = ass.Name;
                if (AttributeMapper.Clasification.TryGetValue(name, out var classification))
                {
                    ass.SetUserProperty("KLASIFIKACIJA", classification);
                    ass.Modify();
                }
                else
                {
                    if (!errList.Contains(name)) errList.Add(name);
                }
            }
            else if (element is Part part)
            {
                var name = part.Name;
                if (AttributeMapper.Clasification.TryGetValue(name, out var classification))
                {
                    part.SetUserProperty("KLASIFIKACIJA", classification);
                    part.Modify();
                }
                else
                {
                    if (!errList.Contains(name)) errList.Add(name);
                }
            }
            else
            {
                throw new NotSupportedException($"This type is not supported {element.GetType()}");
            }
        }
        StringBuilder sb = new StringBuilder();
        foreach (var err in errList)
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
        var polyBeam = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.POLYBEAM).ToList();
        var plates = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE).ToList();
        var assemblies = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY).ToList();
        var rebars = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.REBARGROUP).ToList();
        var rebars2 = mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.SINGLEREBAR).ToList();
        rebars.AddRange(rebars2);
        var combo = beams.Concat(plates).Concat(assemblies).Concat(polyBeam);
        foreach (var part in combo)
        {
            var nm = part.GetStringProp("NAME");
            if(nm == "Sideplate1" || nm == "")
            {
                var o = 0;
            }
            CreateAttributes(part);
        }
        foreach (var rebar in rebars)
        {
            if (rebar is Reinforcement reinforcement)
            {
                CreateAttributesForRebar(reinforcement);
            }
        }
        model.CommitChanges();
    }

    private void AddProperty(Part part, string name, string value)
    {
        if (value == "" || value is null)
        {
            part.SetUserProperty(name, "N/A");
        }
        else
        {
            part.SetUserProperty(name, value);
        }
    }
    private void AddProperty(Part part, string name, double value)
    {
        if (value == 0)
        {
            part.SetUserProperty(name, "N/A");
        }
        else
        {
            part.SetUserProperty(name, value);
        }
    }
    private void AddProperty(Assembly ass, string name, string value)
    {
        if (value == "" || value is null)
        {
            ass.SetUserProperty(name, "N/A");
        }
        else
        {
            ass.SetUserProperty(name, value);
        }
    }
    private void AddProperty(Assembly ass, string name, double value)
    {
        if (value == 0)
        {
            ass.SetUserProperty(name, "N/A");
        }
        else
        {
            ass.SetUserProperty(name, value);
        }
    }

    private void SetName(Part part,string name)
    {
        if (_petraParts.Contains(name))
        {
            part.SetUserProperty("NAME", "PETRA");
        }
        else
        {
            part.SetUserProperty("NAME", name);
        }
    }
    private (bool IsNumber,string Profile) GetSection(string name)
    {
        var parts = name.Split('*');
        if (!(parts.Length == 2)) return (false,name);
        var firstIsNumber = double.TryParse(parts[0],out double num1);
        var secondIsNumber = double.TryParse(parts[1],out double num2);
        bool isNumber = firstIsNumber && secondIsNumber;
        if (isNumber)
        {
            num1 = Math.Round(num1,0);
            num2 = Math.Round(num2,0);
            return (true, $"{num1}*{num2}");
        }
        else
        {
            return (false,name);
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
        { "KOOLTHERM K20", "IZOLĀCIJA" },
        { "C30/37 SBB", "DZELZSBETONS"}
    };
    public static Dictionary<string, string> MaterialFromNames { get; set; } = new Dictionary<string, string>
    {
        {"SIENAS PANELIS", "DZLEZSBETONA PANELIS AR SILTUMIZOLĀCIJU" }
    };
    //Classification 
    public static Dictionary<string, string> Clasification { get; set; } = new Dictionary<string, string>
    {
        {"IZOLĀCIJA", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas" },
        {"NESOŠAIS SLĀNIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"APDARES ĶIEĢELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"APDARES SLĀNIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"SILTUMIZOLĀCIJA", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"SIENAS PANELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"VIENSLĀŅU SIENAS PANELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"TRĪSSLĀŅU SIENAS PANELIS", "BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas"},
        {"PĀRSEGUMA PANELIS", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"SMALKGRAUDAINS BETONS", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"PETRA", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"Sideplate1", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"Sideplate1st", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"Plate1", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"Plate2", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"Roundbar1", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"PĀRSEGUMA JOSLA", "BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi"},
        {"IELIEKAMĀ DETAĻA", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"BULTSKRŪVE M12x100, 8.8", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"UZGRIEZNIS M16", "BE_07_33_01_00_Mehāniski stiprinājumi"},
        {"PAPLĀKSNE M16", "BE_07_33_01_00_Mehāniski stiprinājumi"},
        {"HAS-U+HIT-HY 200-A", "BE_07_33_03_00_Ķīmiski stiprinājumi"},
        {"RVT-M12x50", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"WELDA100x100-108", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"TSS 101", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"PLĀKSNE", "BE_07_13_05_00_Tērauda kolonnas"},
        {"METĀLA SIJA", "BE_07_21_05_00_Tērauda sijas"},
        {"JUMTA SIJA", "BE_07_21_05_00_Tērauda sijas"},
        {"KOLONNA", "BE_07_13_05_00_Tērauda kolonnas"},
        {"METĀLA KOLONNA", "BE_07_13_05_00_Tērauda kolonnas"},
        {"HORIZONTĀLĀ SAITE","BE_07_27_05_00_Tērauda saites"},
        {"VĒJA SAITE","BE_07_27_05_00_Tērauda saites"},
        {"KĀPŅU SIJA","BE_07_29_05_00_Tērauda kāpnes un pandusi"},
        {"PAKĀPIENS","BE_07_29_05_00_Tērauda kāpnes un pandusi"},
        {"KĀPŅU LAIDS","BE_07_29_03_00_Saliekamā dzelzsbetona (SDZB) kāpnes un pandusi"},
        {"KĀPŅU LAUKUMS","BE_07_29_03_00_Saliekamā dzelzsbetona (SDZB) kāpnes un pandusi"},
        {"METINĀTS PLATFORMU REŽĢIS","BE_07_29_05_00_Tērauda kāpnes un pandusi"},
        {"RVL100", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"SCHOCK DORN SLD 50", "BE_07_33_07_00_Iebetonējami stiprinājumi"},
        {"PAMATU PLĀTNE", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
        {"KĀPŅU PAMATS", "BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati"},
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
/*
BE_01_01_00_00	_01	_01	_00	_00	Būvlaukuma nožogojums	BE_01_01_00_00_Būvlaukuma nožogojums
BE_01_01_01_00	_01	_01	_01	_00	Žogi	BE_01_01_01_00_Žogi
BE_01_01_03_00	_01	_01	_03	_00	Vārti	BE_01_01_03_00_Vārti
BE_01_01_05_00	_01	_01	_05	_00	Barjeras	BE_01_01_05_00_Barjeras
BE_01_01_07_00	_01	_01	_07	_00	Turniketi	BE_01_01_07_00_Turniketi
BE_01_03_00_00	_01	_03	_00	_00	Esošās apbūves un koku aizsardzība	BE_01_03_00_00_Esošās apbūves un koku aizsardzība
BE_01_03_01_00	_01	_03	_01	_00	Esošo būvju aizsargsistēmas	BE_01_03_01_00_Esošo būvju aizsargsistēmas
BE_01_03_03_00	_01	_03	_03	_00	Esošo inženierkomunikāciju aizsargsistēmas	BE_01_03_03_00_Esošo inženierkomunikāciju aizsargsistēmas
BE_01_03_05_00	_01	_03	_05	_00	Koku un koku sakņu aizsargsistēmas	BE_01_03_05_00_Koku un koku sakņu aizsargsistēmas
BE_01_05_00_00	_01	_05	_00	_00	Pagaidu segumi	BE_01_05_00_00_Pagaidu segumi
BE_01_05_01_00	_01	_05	_01	_00	Pagaidu iebrauktuve	BE_01_05_01_00_Pagaidu iebrauktuve
BE_01_05_03_00	_01	_05	_03	_00	Pagaidu ceļi	BE_01_05_03_00_Pagaidu ceļi
BE_01_05_05_00	_01	_05	_05	_00	Pagaidu autosstāvieta	BE_01_05_05_00_Pagaidu autosstāvieta
BE_01_05_07_00	_01	_05	_07	_00	Būvmateriālu nokraušanas laukumi	BE_01_05_07_00_Būvmateriālu nokraušanas laukumi
BE_01_05_09_00	_01	_05	_09	_00	Beramo būvmateriālu nokraušanas laukums	BE_01_05_09_00_Beramo būvmateriālu nokraušanas laukums
BE_01_05_11_00	_01	_05	_11	_00	Riteņu mazgāšanas laukums	BE_01_05_11_00_Riteņu mazgāšanas laukums
BE_01_07_00_00	_01	_07	_00	_00	Pilsētiņas izveide	BE_01_07_00_00_Pilsētiņas izveide
BE_01_07_01_00	_01	_07	_01	_00	Būvlaukuma vadības telpas	BE_01_07_01_00_Būvlaukuma vadības telpas
BE_01_07_03_00	_01	_07	_03	_00	Strādnieku telpas	BE_01_07_03_00_Strādnieku telpas
BE_01_07_05_00	_01	_07	_05	_00	Noliktavas telpas	BE_01_07_05_00_Noliktavas telpas
BE_01_07_07_00	_01	_07	_07	_00	Apsardzes telpas	BE_01_07_07_00_Apsardzes telpas
BE_01_07_09_00	_01	_07	_09	_00	Sanitātās telpas	BE_01_07_09_00_Sanitātās telpas
BE_01_07_11_00	_01	_07	_11	_00	Tualetes	BE_01_07_11_00_Tualetes
BE_01_07_13_00	_01	_07	_13	_00	Smēķēšanas vietas	BE_01_07_13_00_Smēķēšanas vietas
BE_01_09_00_00	_01	_09	_00	_00	Būvlaukuma aprīkojums	BE_01_09_00_00_Būvlaukuma aprīkojums
BE_01_09_01_00	_01	_09	_01	_00	Sastatņu sistēmas	BE_01_09_01_00_Sastatņu sistēmas
BE_01_09_03_00	_01	_09	_03	_00	Kustību organizācijas sistēmas	BE_01_09_03_00_Kustību organizācijas sistēmas
BE_01_09_05_00	_01	_09	_05	_00	Darba drošības aprīkojums	BE_01_09_05_00_Darba drošības aprīkojums
BE_01_09_07_00	_01	_09	_07	_00	Ugunsdrošības aprīkojums	BE_01_09_07_00_Ugunsdrošības aprīkojums
BE_01_09_09_00	_01	_09	_09	_00	Informatīvais aprīkojums	BE_01_09_09_00_Informatīvais aprīkojums
BE_01_09_11_00	_01	_09	_11	_00	Torņa celtņi un to aprīkojums	BE_01_09_11_00_Torņa celtņi un to aprīkojums
BE_01_09_13_00	_01	_09	_13	_00	Būvgružu konteineri	BE_01_09_13_00_Būvgružu konteineri
BE_01_11_00_00	_01	_11	_00	_00	Pagaidu inženiertīkli	BE_01_11_00_00_Pagaidu inženiertīkli
BE_01_11_01_00	_01	_11	_01	_00	Pagaidu elektroapgāde	BE_01_11_01_00_Pagaidu elektroapgāde
BE_01_11_03_00	_01	_11	_03	_00	Pagaidu apgaismojums	BE_01_11_03_00_Pagaidu apgaismojums
BE_01_11_05_00	_01	_11	_05	_00	Pagaidu apsardzes signalizācija	BE_01_11_05_00_Pagaidu apsardzes signalizācija
BE_01_11_07_00	_01	_11	_07	_00	Pagaidu ūdensapgāde	BE_01_11_07_00_Pagaidu ūdensapgāde
BE_01_11_09_00	_01	_11	_09	_00	Pagaidu notekūdeņu novadīšana	BE_01_11_09_00_Pagaidu notekūdeņu novadīšana
BE_01_13_00_00	_01	_13	_00	_00	Monitorings un uzraudzība	BE_01_13_00_00_Monitorings un uzraudzība
BE_01_13_01_00	_01	_13	_01	_00	Esošo ēku monitorings	BE_01_13_01_00_Esošo ēku monitorings
BE_01_13_03_00	_01	_13	_03	_00	Koku monitorings	BE_01_13_03_00_Koku monitorings
BE_01_13_05_00	_01	_13	_05	_00	Būvlaukuma video monitorings	BE_01_13_05_00_Būvlaukuma video monitorings
BE_01_13_07_00	_01	_13	_07	_00	Ģeotehniskā uzraudzība	BE_01_13_07_00_Ģeotehniskā uzraudzība
BE_01_13_09_00	_01	_13	_09	_00	Ģeodēziskā uzraudzība	BE_01_13_09_00_Ģeodēziskā uzraudzība
BE_01_13_11_00	_01	_13	_11	_00	Strādnieku elektroniskā darba laika uzskaites sistēma	BE_01_13_11_00_Strādnieku elektroniskā darba laika uzskaites sistēma
BE_03_00_00_00	_03	_00	_00	_00	Demontējamās sistēmas	BE_03_00_00_00_Demontējamās sistēmas
BE_03_01_00_00	_03	_01	_00	_00	Demontējamās konstrukcijas	BE_03_01_00_00_Demontējamās konstrukcijas
BE_03_03_00_00	_03	_03	_00	_00	Demontējamie iekšējie inženiertīkli	BE_03_03_00_00_Demontējamie iekšējie inženiertīkli
BE_03_05_00_00	_03	_05	_00	_00	Demontējamie teritorijas elementi	BE_03_05_00_00_Demontējamie teritorijas elementi
BE_03_07_00_00	_03	_07	_00	_00	Demontējamās ailes un atvērumi	BE_03_07_00_00_Demontējamās ailes un atvērumi
BE_03_09_00_00	_03	_09	_00	_00	Cērtamie koki un krūmi	BE_03_09_00_00_Cērtamie koki un krūmi
BE_03_11_00_00	_03	_11	_00	_00	Citas demontējamās sistēmas	BE_03_11_00_00_Citas demontējamās sistēmas
BE_05_00_00_00	_05	_00	_00	_00	Zemes darbi	BE_05_00_00_00_Zemes darbi
BE_05_01_00_00	_05	_01	_00	_00	Ūdens samazināšana	BE_05_01_00_00_Ūdens samazināšana
BE_05_03_00_00	_05	_03	_00	_00	Grunts stabilizācija	BE_05_03_00_00_Grunts stabilizācija
BE_05_03_01_00	_05	_03	_01	_00	Rievsienas	BE_05_03_01_00_Rievsienas
BE_05_03_03_00	_05	_03	_03	_00	Atbalstsienas	BE_05_03_03_00_Atbalstsienas
BE_05_03_05_00	_05	_03	_05	_00	Gravitācijas siena	BE_05_03_05_00_Gravitācijas siena
BE_05_03_07_00	_05	_03	_07	_00	Berlīnes tipa atbalstsiena	BE_05_03_07_00_Berlīnes tipa atbalstsiena
BE_05_03_09_00	_05	_03	_09	_00	Grunts enkuri, naglas	BE_05_03_09_00_Grunts enkuri, naglas
BE_05_05_00_00	_05	_05	_00	_00	Norokamā grunts	BE_05_05_00_00_Norokamā grunts
BE_05_05_01_00	_05	_05	_01	_00	Noņemā augsnes kārta	BE_05_05_01_00_Noņemā augsnes kārta
BE_05_05_03_00	_05	_05	_03	_00	Izņemamā piesārņotā grunts	BE_05_05_03_00_Izņemamā piesārņotā grunts
BE_05_05_05_00	_05	_05	_05	_00	Izņemamā nederīgā grunts	BE_05_05_05_00_Izņemamā nederīgā grunts
BE_05_05_07_00	_05	_05	_07	_00	Uz atbērtni pārvietojamā grunts	BE_05_05_07_00_Uz atbērtni pārvietojamā grunts
BE_07_00_00_00	_07	_00	_00	_00	Būvkonstrukcijas (BK)	BE_07_00_00_00_Būvkonstrukcijas (BK)
BE_07_01_00_00	_07	_01	_00	_00	Pamatne	BE_07_01_00_00_Pamatne
BE_07_01_01_00	_07	_01	_01	_00	Pieberama pamatne	BE_07_01_01_00_Pieberama pamatne
BE_07_01_03_00	_07	_01	_03	_00	Ieklājama, betonējama pamatne	BE_07_01_03_00_Ieklājama, betonējama pamatne
BE_07_01_05_00	_07	_01	_05	_00	Cita pamatne	BE_07_01_05_00_Cita pamatne
BE_07_03_00_00	_07	_03	_00	_00	Grunts pastiprināšana	BE_07_03_00_00_Grunts pastiprināšana
BE_07_03_01_00	_07	_03	_01	_00	Cementēšana	BE_07_03_01_00_Cementēšana
BE_07_03_03_00	_07	_03	_03	_00	Dinamiskā blīvēšana	BE_07_03_03_00_Dinamiskā blīvēšana
BE_07_03_05_00	_07	_03	_05	_00	Grunts pāļi	BE_07_03_05_00_Grunts pāļi
BE_07_03_07_00	_07	_03	_07	_00	Grunts konsolidācija	BE_07_03_07_00_Grunts konsolidācija
BE_07_03_09_00	_07	_03	_09	_00	Vibroflotācija	BE_07_03_09_00_Vibroflotācija
BE_07_03_11_00	_07	_03	_11	_00	Cita pastiprināšana	BE_07_03_11_00_Cita pastiprināšana
BE_07_05_00_00	_07	_05	_00	_00	Pāļi	BE_07_05_00_00_Pāļi
BE_07_05_01_00	_07	_05	_01	_00	Urbtie pāļi	BE_07_05_01_00_Urbtie pāļi
BE_07_05_03_00	_07	_05	_03	_00	Dzītie pāļi	BE_07_05_03_00_Dzītie pāļi
BE_07_05_05_00	_07	_05	_05	_00	Skrūvpāļi	BE_07_05_05_00_Skrūvpāļi
BE_07_05_07_00	_07	_05	_07	_00	Mikropāļi	BE_07_05_07_00_Mikropāļi
BE_07_05_09_00	_07	_05	_09	_00	Citi pāļi	BE_07_05_09_00_Citi pāļi
BE_07_07_00_00	_07	_07	_00	_00	Pamati	BE_07_07_00_00_Pamati
BE_07_07_01_00	_07	_07	_01	_00	Monolītā dzelzsbetona (MDZB) pamati	BE_07_07_01_00_Monolītā dzelzsbetona (MDZB) pamati
BE_07_07_03_00	_07	_07	_03	_00	Saliekamā dzelzsbetona (SDZB) pamati	BE_07_07_03_00_Saliekamā dzelzsbetona (SDZB) pamati
BE_07_07_05_00	_07	_07	_05	_00	Tērauda pamati	BE_07_07_05_00_Tērauda pamati
BE_07_07_07_00	_07	_07	_07	_00	Mūra pamati	BE_07_07_07_00_Mūra pamati
BE_07_07_09_00	_07	_07	_09	_00	Citi pamati	BE_07_07_09_00_Citi pamati
BE_07_09_00_00	_07	_09	_00	_00	Speciālie pamati	BE_07_09_00_00_Speciālie pamati
BE_07_09_01_00	_07	_09	_01	_00	Kesoni	BE_07_09_01_00_Kesoni
BE_07_09_03_00	_07	_09	_03	_00	Citi speciālie pamati	BE_07_09_03_00_Citi speciālie pamati
BE_07_11_00_00	_07	_11	_00	_00	Kolonnu bāzes	BE_07_11_00_00_Kolonnu bāzes
BE_07_11_01_00	_07	_11	_01	_00	Monolītā dzelzsbetona (MDZB) kolonnu bāzes	BE_07_11_01_00_Monolītā dzelzsbetona (MDZB) kolonnu bāzes
BE_07_11_03_00	_07	_11	_03	_00	Tērauda kolonnu bāzes	BE_07_11_03_00_Tērauda kolonnu bāzes
BE_07_13_00_00	_07	_13	_00	_00	Kolonnas	BE_07_13_00_00_Kolonnas
BE_07_13_01_00	_07	_13	_01	_00	Monolītā dzelzsbetona (MDZB) kolonnas	BE_07_13_01_00_Monolītā dzelzsbetona (MDZB) kolonnas
BE_07_13_03_00	_07	_13	_03	_00	Saliekamā dzelzsbetona (SDZB) kolonnas	BE_07_13_03_00_Saliekamā dzelzsbetona (SDZB) kolonnas
BE_07_13_05_00	_07	_13	_05	_00	Tērauda kolonnas	BE_07_13_05_00_Tērauda kolonnas
BE_07_13_07_00	_07	_13	_07	_00	Tēraudbetona kolonnas	BE_07_13_07_00_Tēraudbetona kolonnas
BE_07_13_09_00	_07	_13	_09	_00	Koka kolonnas	BE_07_13_09_00_Koka kolonnas
BE_07_13_11_00	_07	_13	_11	_00	Mūra kolonnas	BE_07_13_11_00_Mūra kolonnas
BE_07_13_13_00	_07	_13	_13	_00	Alumīnija kolonnas	BE_07_13_13_00_Alumīnija kolonnas
BE_07_13_15_00	_07	_13	_15	_00	Kompozīta kolonnas	BE_07_13_15_00_Kompozīta kolonnas
BE_07_13_17_00	_07	_13	_17	_00	Stikla kolonnas	BE_07_13_17_00_Stikla kolonnas
BE_07_13_19_00	_07	_13	_19	_00	Citas kolonnas	BE_07_13_19_00_Citas kolonnas
BE_07_15_00_00	_07	_15	_00	_00	Sienas	BE_07_15_00_00_Sienas
BE_07_15_01_00	_07	_15	_01	_00	Monolītā dzelzsbetona (MDZB) sienas	BE_07_15_01_00_Monolītā dzelzsbetona (MDZB) sienas
BE_07_15_03_00	_07	_15	_03	_00	Saliekamā dzelzsbetona (SDZB) sienas	BE_07_15_03_00_Saliekamā dzelzsbetona (SDZB) sienas
BE_07_15_05_00	_07	_15	_05	_00	Koka sienas	BE_07_15_05_00_Koka sienas
BE_07_15_07_00	_07	_15	_07	_00	Krusteniski līmēta koka (CLT) sienas	BE_07_15_07_00_Krusteniski līmēta koka (CLT) sienas
BE_07_15_09_00	_07	_15	_09	_00	Mūra sienas	BE_07_15_09_00_Mūra sienas
BE_07_15_11_00	_07	_15	_11	_00	Kompozīta sienas	BE_07_15_11_00_Kompozīta sienas
BE_07_15_13_00	_07	_15	_13	_00	Citas sienas	BE_07_15_13_00_Citas sienas
BE_07_17_00_00	_07	_17	_00	_00	Pārsedzes	BE_07_17_00_00_Pārsedzes
BE_07_17_01_00	_07	_17	_01	_00	Monolītā dzelzsbetona (MDZB) pārsedzes	BE_07_17_01_00_Monolītā dzelzsbetona (MDZB) pārsedzes
BE_07_17_03_00	_07	_17	_03	_00	Saliekamā dzelzsbetona (SDZB) pārsedzes	BE_07_17_03_00_Saliekamā dzelzsbetona (SDZB) pārsedzes
BE_07_17_05_00	_07	_17	_05	_00	Tērauda pārsedzes	BE_07_17_05_00_Tērauda pārsedzes
BE_07_17_07_00	_07	_17	_07	_00	Koka pārsedzes	BE_07_17_07_00_Koka pārsedzes
BE_07_17_09_00	_07	_17	_09	_00	Mūra pārsedzes	BE_07_17_09_00_Mūra pārsedzes
BE_07_17_11_00	_07	_17	_11	_00	Kompozīta pārsedzes	BE_07_17_11_00_Kompozīta pārsedzes
BE_07_17_13_00	_07	_17	_13	_00	Citas pārsedzes	BE_07_17_13_00_Citas pārsedzes
BE_07_19_00_00	_07	_19	_00	_00	Pārsegumi	BE_07_19_00_00_Pārsegumi
BE_07_19_01_00	_07	_19	_01	_00	Monolītā dzelzsbetona (MDZB) pārsegumi	BE_07_19_01_00_Monolītā dzelzsbetona (MDZB) pārsegumi
BE_07_19_03_00	_07	_19	_03	_00	Saliekamā dzelzsbetona (SDZB) pārsegumi	BE_07_19_03_00_Saliekamā dzelzsbetona (SDZB) pārsegumi
BE_07_19_05_00	_07	_19	_05	_00	Tērauda pārsegumi	BE_07_19_05_00_Tērauda pārsegumi
BE_07_19_07_00	_07	_19	_07	_00	Tēraudbetona pārsegumi	BE_07_19_07_00_Tēraudbetona pārsegumi
BE_07_19_09_00	_07	_19	_09	_00	Koka pārsegumi	BE_07_19_09_00_Koka pārsegumi
BE_07_19_11_00	_07	_19	_11	_00	Krusteniski līmēta koka (CLT) pārsegumi	BE_07_19_11_00_Krusteniski līmēta koka (CLT) pārsegumi
BE_07_19_13_00	_07	_19	_13	_00	Alumīnija pārsegumi	BE_07_19_13_00_Alumīnija pārsegumi
BE_07_19_15_00	_07	_19	_15	_00	Kompozīta pārsegumi	BE_07_19_15_00_Kompozīta pārsegumi
BE_07_19_17_00	_07	_19	_17	_00	Stikla pārsegumi	BE_07_19_17_00_Stikla pārsegumi
BE_07_19_19_00	_07	_19	_19	_00	Citi pārsegumi	BE_07_19_19_00_Citi pārsegumi
BE_07_21_00_00	_07	_21	_00	_00	Sijas	BE_07_21_00_00_Sijas
BE_07_21_01_00	_07	_21	_01	_00	Monolītā dzelzsbetona (MDZB) sijas	BE_07_21_01_00_Monolītā dzelzsbetona (MDZB) sijas
BE_07_21_03_00	_07	_21	_03	_00	Saliekamā dzelzsbetona (SDZB) sijas	BE_07_21_03_00_Saliekamā dzelzsbetona (SDZB) sijas
BE_07_21_05_00	_07	_21	_05	_00	Tērauda sijas	BE_07_21_05_00_Tērauda sijas
BE_07_21_07_00	_07	_21	_07	_00	Tēraudbetona sijas	BE_07_21_07_00_Tēraudbetona sijas
BE_07_21_09_00	_07	_21	_09	_00	Koka sijas	BE_07_21_09_00_Koka sijas
BE_07_21_11_00	_07	_21	_11	_00	Alumīnija sijas	BE_07_21_11_00_Alumīnija sijas
BE_07_21_13_00	_07	_21	_13	_00	Kompozīta sijas	BE_07_21_13_00_Kompozīta sijas
BE_07_21_15_00	_07	_21	_15	_00	Stikla sijas	BE_07_21_15_00_Stikla sijas
BE_07_21_17_00	_07	_21	_17	_00	Citas sijas	BE_07_21_17_00_Citas sijas
BE_07_23_00_00	_07	_23	_00	_00	Kopnes, režģotas konstrukcijas	BE_07_23_00_00_Kopnes, režģotas konstrukcijas
BE_07_23_01_00	_07	_23	_01	_00	Monolītā dzelzsbetona (MDZB) kopnes, režģotas konstrukcijas	BE_07_23_01_00_Monolītā dzelzsbetona (MDZB) kopnes, režģotas konstrukcijas
BE_07_23_03_00	_07	_23	_03	_00	Saliekamā dzelzsbetona (SDZB) kopnes, režģotas konstrukcijas	BE_07_23_03_00_Saliekamā dzelzsbetona (SDZB) kopnes, režģotas konstrukcijas
BE_07_23_05_00	_07	_23	_05	_00	Tērauda kopnes, režģotas konstrukcijas	BE_07_23_05_00_Tērauda kopnes, režģotas konstrukcijas
BE_07_23_07_00	_07	_23	_07	_00	Tēraudbetona kopnes, režģotas konstrukcijas	BE_07_23_07_00_Tēraudbetona kopnes, režģotas konstrukcijas
BE_07_23_09_00	_07	_23	_09	_00	Koka kopnes, režģotas konstrukcijas	BE_07_23_09_00_Koka kopnes, režģotas konstrukcijas
BE_07_23_11_00	_07	_23	_11	_00	Alumīnija kopnes, režģotas konstrukcijas	BE_07_23_11_00_Alumīnija kopnes, režģotas konstrukcijas
BE_07_23_13_00	_07	_23	_13	_00	Kompozīta kopnes, režģotas konstrukcijas	BE_07_23_13_00_Kompozīta kopnes, režģotas konstrukcijas
BE_07_23_15_00	_07	_23	_15	_00	Citas kopnes, režģotas konstrukcijas	BE_07_23_15_00_Citas kopnes, režģotas konstrukcijas
BE_07_25_00_00	_07	_25	_00	_00	Čaulas un membrānas	BE_07_25_00_00_Čaulas un membrānas
BE_07_25_01_00	_07	_25	_01	_00	Monolītā dzelzsbetona (MDZB) čaulas	BE_07_25_01_00_Monolītā dzelzsbetona (MDZB) čaulas
BE_07_25_03_00	_07	_25	_03	_00	Saliekamā dzelzsbetona (SDZB) čaulas	BE_07_25_03_00_Saliekamā dzelzsbetona (SDZB) čaulas
BE_07_25_05_00	_07	_25	_05	_00	Tērauda čaulas	BE_07_25_05_00_Tērauda čaulas
BE_07_25_07_00	_07	_25	_07	_00	Tēraudbetona čaulas	BE_07_25_07_00_Tēraudbetona čaulas
BE_07_25_09_00	_07	_25	_09	_00	Koka čaulas	BE_07_25_09_00_Koka čaulas
BE_07_25_11_00	_07	_25	_11	_00	Krusteniski līmēta koka (CLT) čaulas	BE_07_25_11_00_Krusteniski līmēta koka (CLT) čaulas
BE_07_25_13_00	_07	_25	_13	_00	Alumīnija čaulas	BE_07_25_13_00_Alumīnija čaulas
BE_07_25_15_00	_07	_25	_15	_00	Kompozīta čaulas	BE_07_25_15_00_Kompozīta čaulas
BE_07_25_17_00	_07	_25	_17	_00	Stikla čaulas	BE_07_25_17_00_Stikla čaulas
BE_07_25_19_00	_07	_25	_19	_00	Citas čaulas	BE_07_25_19_00_Citas čaulas
BE_07_27_00_00	_07	_27	_00	_00	Saites un troses	BE_07_27_00_00_Saites un troses
BE_07_27_01_00	_07	_27	_01	_00	Monolītā dzelzsbetona (MDZB) saites	BE_07_27_01_00_Monolītā dzelzsbetona (MDZB) saites
BE_07_27_03_00	_07	_27	_03	_00	Saliekamā dzelzsbetona (SDZB) saites	BE_07_27_03_00_Saliekamā dzelzsbetona (SDZB) saites
BE_07_27_05_00	_07	_27	_05	_00	Tērauda saites	BE_07_27_05_00_Tērauda saites
BE_07_27_07_00	_07	_27	_07	_00	Koka saites	BE_07_27_07_00_Koka saites
BE_07_27_09_00	_07	_27	_09	_00	Kompozīta saites	BE_07_27_09_00_Kompozīta saites
BE_07_27_11_00	_07	_27	_11	_00	Citas saites	BE_07_27_11_00_Citas saites
BE_07_29_00_00	_07	_29	_00	_00	Kāpnes un pandusi	BE_07_29_00_00_Kāpnes un pandusi
BE_07_29_01_00	_07	_29	_01	_00	Monolītā dzelzsbetona (MDZB) kāpnes un pandusi	BE_07_29_01_00_Monolītā dzelzsbetona (MDZB) kāpnes un pandusi
BE_07_29_03_00	_07	_29	_03	_00	Saliekamā dzelzsbetona (SDZB) kāpnes un pandusi	BE_07_29_03_00_Saliekamā dzelzsbetona (SDZB) kāpnes un pandusi
BE_07_29_05_00	_07	_29	_05	_00	Tērauda kāpnes un pandusi	BE_07_29_05_00_Tērauda kāpnes un pandusi
BE_07_29_07_00	_07	_29	_07	_00	Tēraudbetona kāpnes un pandusi	BE_07_29_07_00_Tēraudbetona kāpnes un pandusi
BE_07_29_09_00	_07	_29	_09	_00	Koka kāpnes un pandusi	BE_07_29_09_00_Koka kāpnes un pandusi
BE_07_29_11_00	_07	_29	_11	_00	Alumīnija kāpnes un pandusi	BE_07_29_11_00_Alumīnija kāpnes un pandusi
BE_07_29_13_00	_07	_29	_13	_00	Kompozīta kāpnes un pandusi	BE_07_29_13_00_Kompozīta kāpnes un pandusi
BE_07_29_15_00	_07	_29	_15	_00	Stikla kāpnes un pandusi	BE_07_29_15_00_Stikla kāpnes un pandusi
BE_07_29_17_00	_07	_29	_17	_00	Citas kāpnes un pandusi	BE_07_29_17_00_Citas kāpnes un pandusi
BE_07_31_00_00	_07	_31	_00	_00	Iekārtu, AR apakškonstrukcijas	BE_07_31_00_00_Iekārtu, AR apakškonstrukcijas
BE_07_31_01_00	_07	_31	_01	_00	Iekārtu rāmji, apakškonstrukcijas 	BE_07_31_01_00_Iekārtu rāmji, apakškonstrukcijas 
BE_07_31_03_00	_07	_31	_03	_00	Citas apakškonstrukcijas 	BE_07_31_03_00_Citas apakškonstrukcijas 
BE_07_33_00_00	_07	_33	_00	_00	Stiprinājumi	BE_07_33_00_00_Stiprinājumi
BE_07_33_01_00	_07	_33	_01	_00	Mehāniski stiprinājumi	BE_07_33_01_00_Mehāniski stiprinājumi
BE_07_33_03_00	_07	_33	_03	_00	Ķīmiski stiprinājumi	BE_07_33_03_00_Ķīmiski stiprinājumi
BE_07_33_05_00	_07	_33	_05	_00	Metināti stiprinājumi	BE_07_33_05_00_Metināti stiprinājumi
BE_07_33_07_00	_07	_33	_07	_00	Iebetonējami stiprinājumi	BE_07_33_07_00_Iebetonējami stiprinājumi
BE_07_35_00_00	_07	_35	_00	_00	Šuves un to materiāli	BE_07_35_00_00_Šuves un to materiāli
BE_07_35_01_00	_07	_35	_01	_00	Deformācijas šuves un to materiāli	BE_07_35_01_00_Deformācijas šuves un to materiāli
BE_07_35_03_00	_07	_35	_03	_00	Hidroizolācijas šuves un to materiāli	BE_07_35_03_00_Hidroizolācijas šuves un to materiāli
BE_07_35_05_00	_07	_35	_05	_00	Termoizolācijas šuves un to materiāli	BE_07_35_05_00_Termoizolācijas šuves un to materiāli
BE_07_35_07_00	_07	_35	_07	_00	Skaņas izolācijas šuves un to materiāli	BE_07_35_07_00_Skaņas izolācijas šuves un to materiāli
BE_07_35_09_00	_07	_35	_09	_00	Spriegumu izlīdzināšanas šuves un to materiāli	BE_07_35_09_00_Spriegumu izlīdzināšanas šuves un to materiāli
BE_07_35_11_00	_07	_35	_11	_00	Darba šuves un to materiāli	BE_07_35_11_00_Darba šuves un to materiāli
BE_07_37_00_00	_07	_37	_00	_00	Pagaidu konstrukcijas	BE_07_37_00_00_Pagaidu konstrukcijas
BE_07_37_01_00	_07	_37	_01	_00	Pagaidu stutes	BE_07_37_01_00_Pagaidu stutes
BE_07_37_03_00	_07	_37	_03	_00	Pagaidu atgāžņi	BE_07_37_03_00_Pagaidu atgāžņi
BE_07_37_05_00	_07	_37	_05	_00	Pagaidu rāmji	BE_07_37_05_00_Pagaidu rāmji
BE_09_00_00_00	_09	_00	_00	_00	Arhitektūras risinājumi (AR)	BE_09_00_00_00_Arhitektūras risinājumi (AR)
BE_09_01_00_00	_09	_01	_00	_00	Grīdas	BE_09_01_00_00_Grīdas
BE_09_01_01_00	_09	_01	_01	_00	Grīdas uz grunts	BE_09_01_01_00_Grīdas uz grunts
BE_09_01_03_00	_09	_01	_03	_00	Grīdas uz pārseguma	BE_09_01_03_00_Grīdas uz pārseguma
BE_09_01_05_00	_09	_01	_05	_00	Paceltās grīdas	BE_09_01_05_00_Paceltās grīdas
BE_09_01_07_00	_09	_01	_07	_00	Terašu, balkonu grīdas	BE_09_01_07_00_Terašu, balkonu grīdas
BE_09_03_00_00	_09	_03	_00	_00	Griesti	BE_09_03_00_00_Griesti
BE_09_03_01_00	_09	_03	_01	_00	Griesti piekārtie	BE_09_03_01_00_Griesti piekārtie
BE_09_03_03_00	_09	_03	_03	_00	Nostieptie griesti	BE_09_03_03_00_Nostieptie griesti
BE_09_05_00_00	_09	_05	_00	_00	Jumti	BE_09_05_00_00_Jumti
BE_09_05_01_00	_09	_05	_01	_00	Jumta klājs	BE_09_05_01_00_Jumta klājs
BE_09_05_03_00	_09	_05	_03	_00	Jumta pamatne	BE_09_05_03_00_Jumta pamatne
BE_09_05_05_00	_09	_05	_05	_00	Jumtu aprīkojums	BE_09_05_05_00_Jumtu aprīkojums
BE_09_05_07_00	_09	_05	_07	_00	Uzjumteņi	BE_09_05_07_00_Uzjumteņi
BE_09_07_00_00	_09	_07	_00	_00	Ārsienas	BE_09_07_00_00_Ārsienas
BE_09_07_01_00	_09	_07	_01	_00	Karkasa ārsienas 	BE_09_07_01_00_Karkasa ārsienas 
BE_09_07_03_00	_09	_07	_03	_00	Piekārtās ārsienas 	BE_09_07_03_00_Piekārtās ārsienas 
BE_09_07_05_00	_09	_07	_05	_00	Mūrētās ārsienas 	BE_09_07_05_00_Mūrētās ārsienas 
BE_09_07_07_00	_09	_07	_07	_00	Cokolsienas	BE_09_07_07_00_Cokolsienas
BE_09_07_09_00	_09	_07	_09	_00	Pagrabstāva ārsienas	BE_09_07_09_00_Pagrabstāva ārsienas
BE_09_09_00_00	_09	_09	_00	_00	Starpsienas	BE_09_09_00_00_Starpsienas
BE_09_09_01_00	_09	_09	_01	_00	Karkasa starpsienas	BE_09_09_01_00_Karkasa starpsienas
BE_09_09_03_00	_09	_09	_03	_00	Mūra starpsienas	BE_09_09_03_00_Mūra starpsienas
BE_09_09_05_00	_09	_09	_05	_00	Stiklotās starpsienas	BE_09_09_05_00_Stiklotās starpsienas
BE_09_11_00_00	_09	_11	_00	_00	Ailu aizpildījums	BE_09_11_00_00_Ailu aizpildījums
BE_09_11_01_00	_09	_11	_01	_00	Logi	BE_09_11_01_00_Logi
BE_09_11_03_00	_09	_11	_03	_00	Durvis	BE_09_11_03_00_Durvis
BE_09_11_05_00	_09	_11	_05	_00	Lūkas	BE_09_11_05_00_Lūkas
BE_09_11_07_00	_09	_11	_07	_00	Vārti	BE_09_11_07_00_Vārti
BE_09_13_00_00	_09	_13	_00	_00	Fasādes	BE_09_13_00_00_Fasādes
BE_09_13_01_00	_09	_13	_01	_00	Karkasa fasādes	BE_09_13_01_00_Karkasa fasādes
BE_09_13_03_00	_09	_13	_03	_00	Apmestās fasādes	BE_09_13_03_00_Apmestās fasādes
BE_09_13_05_00	_09	_13	_05	_00	Piekārtās fasādes	BE_09_13_05_00_Piekārtās fasādes
BE_09_13_07_00	_09	_13	_07	_00	Mūrētās fasādes	BE_09_13_07_00_Mūrētās fasādes
BE_09_13_09_00	_09	_13	_09	_00	Stiklotās  fasādes sistēmas	BE_09_13_09_00_Stiklotās  fasādes sistēmas
BE_09_13_11_00	_09	_13	_11	_00	Fasāžu aprīkojums	BE_09_13_11_00_Fasāžu aprīkojums
BE_09_15_00_00	_09	_15	_00	_00	Margas	BE_09_15_00_00_Margas
BE_09_15_01_00	_09	_15	_01	_00	Balkonu margas	BE_09_15_01_00_Balkonu margas
BE_09_15_03_00	_09	_15	_03	_00	Jumta margas	BE_09_15_03_00_Jumta margas
BE_09_15_05_00	_09	_15	_05	_00	Kāpņu un pandusu margas	BE_09_15_05_00_Kāpņu un pandusu margas
BE_09_15_07_00	_09	_15	_07	_00	Ātrija margas	BE_09_15_07_00_Ātrija margas
BE_09_17_00_00	_09	_17	_00	_00	Aprīkojums	BE_09_17_00_00_Aprīkojums
BE_09_17_01_00	_09	_17	_01	_00	Lieveņi	BE_09_17_01_00_Lieveņi
BE_09_17_03_00	_09	_17	_03	_00	Krāsnis, kamīni	BE_09_17_03_00_Krāsnis, kamīni
BE_09_17_05_00	_09	_17	_05	_00	Cits aprīkojums	BE_09_17_05_00_Cits aprīkojums
BE_11_00_00_00	_11	_00	_00	_00	Interjers (IN)	BE_11_00_00_00_Interjers (IN)
BE_11_01_00_00	_11	_01	_00	_00	Apdare	BE_11_01_00_00_Apdare
BE_11_01_01_00	_11	_01	_01	_00	Grīdas apdare	BE_11_01_01_00_Grīdas apdare
BE_11_01_03_00	_11	_01	_03	_00	Griestu apdare	BE_11_01_03_00_Griestu apdare
BE_11_01_05_00	_11	_01	_05	_00	Sienu apdare	BE_11_01_05_00_Sienu apdare
BE_11_01_07_00	_11	_01	_07	_00	Ailu apdare	BE_11_01_07_00_Ailu apdare
BE_11_01_09_00	_11	_01	_09	_00	Līstes	BE_11_01_09_00_Līstes
BE_11_01_11_00	_11	_01	_11	_00	Citi apdares elementi	BE_11_01_11_00_Citi apdares elementi
BE_11_03_00_00	_11	_03	_00	_00	Apdares aprīkojums	BE_11_03_00_00_Apdares aprīkojums
BE_11_03_01_00	_11	_03	_01	_00	Grīdas aprīkojums	BE_11_03_01_00_Grīdas aprīkojums
BE_11_03_03_00	_11	_03	_03	_00	Griestu aprīkojums	BE_11_03_03_00_Griestu aprīkojums
BE_11_03_05_00	_11	_03	_05	_00	Sienu aprīkojums	BE_11_03_05_00_Sienu aprīkojums
BE_11_03_07_00	_11	_03	_07	_00	Ailu aprīkojums	BE_11_03_07_00_Ailu aprīkojums
BE_11_05_00_00	_11	_05	_00	_00	Telpu aprīkojums	BE_11_05_00_00_Telpu aprīkojums
BE_11_05_01_00	_11	_05	_01	_00	Sanmezgla aprīkojums	BE_11_05_01_00_Sanmezgla aprīkojums
BE_11_05_03_00	_11	_05	_03	_00	Virtuves aprīkojums	BE_11_05_03_00_Virtuves aprīkojums
BE_11_05_05_00	_11	_05	_05	_00	Biroja telpu aprīkojums	BE_11_05_05_00_Biroja telpu aprīkojums
BE_11_05_07_00	_11	_05	_07	_00	Dzīvojamās telpas aprīkojums	BE_11_05_07_00_Dzīvojamās telpas aprīkojums
BE_11_05_09_00	_11	_05	_09	_00	Tehnisko telpu aprīkojums	BE_11_05_09_00_Tehnisko telpu aprīkojums
BE_11_05_11_00	_11	_05	_11	_00	Komerctelpu aprīkojums	BE_11_05_11_00_Komerctelpu aprīkojums
BE_11_05_13_00	_11	_05	_13	_00	Citu telpu aprīkojums	BE_11_05_13_00_Citu telpu aprīkojums
BE_11_07_00_00	_11	_07	_00	_00	Mēbeles	BE_11_07_00_00_Mēbeles
BE_11_07_01_00	_11	_07	_01	_00	Sanmezgla mēbeles	BE_11_07_01_00_Sanmezgla mēbeles
BE_11_07_03_00	_11	_07	_03	_00	Virtuves mēbeles	BE_11_07_03_00_Virtuves mēbeles
BE_11_07_05_00	_11	_07	_05	_00	Biroja mēbeles	BE_11_07_05_00_Biroja mēbeles
BE_11_07_07_00	_11	_07	_07	_00	Dzīvojamo telpu mēbeles	BE_11_07_07_00_Dzīvojamo telpu mēbeles
BE_11_07_09_00	_11	_07	_09	_00	Tehnisko telpu mēbeles	BE_11_07_09_00_Tehnisko telpu mēbeles
BE_11_07_11_00	_11	_07	_11	_00	Komerctelpu mēbeles	BE_11_07_11_00_Komerctelpu mēbeles
BE_11_07_13_00	_11	_07	_13	_00	Citu telpu mēbeles	BE_11_07_13_00_Citu telpu mēbeles
BE_11_09_00_00	_11	_09	_00	_00	Tehnika	BE_11_09_00_00_Tehnika
BE_11_09_01_00	_11	_09	_01	_00	Sanmezgla tehnika	BE_11_09_01_00_Sanmezgla tehnika
BE_11_09_03_00	_11	_09	_03	_00	Virtuves tehnika	BE_11_09_03_00_Virtuves tehnika
BE_11_09_05_00	_11	_09	_05	_00	Biroja tehnika	BE_11_09_05_00_Biroja tehnika
BE_11_09_07_00	_11	_09	_07	_00	Dzīvojamo telpu tehnika	BE_11_09_07_00_Dzīvojamo telpu tehnika
BE_11_09_09_00	_11	_09	_09	_00	Tehnisko telpu tehnika	BE_11_09_09_00_Tehnisko telpu tehnika
BE_11_09_11_00	_11	_09	_11	_00	Komerctelpu tehnika	BE_11_09_11_00_Komerctelpu tehnika
BE_11_09_13_00	_11	_09	_13	_00	Citu telpu tehnika	BE_11_09_13_00_Citu telpu tehnika
BE_13_00_00_00	_13	_00	_00	_00	Labiekārtojums un apzaļumošana (TS-L)	BE_13_00_00_00_Labiekārtojums un apzaļumošana (TS-L)
BE_13_01_00_00	_13	_01	_00	_00	Segumi	BE_13_01_00_00_Segumi
BE_13_01_01_00	_13	_01	_01	_00	Oļu segumi	BE_13_01_01_00_Oļu segumi
BE_13_01_03_00	_13	_01	_03	_00	Mulčas segumi	BE_13_01_03_00_Mulčas segumi
BE_13_01_05_00	_13	_01	_05	_00	Auglīgās augsnes segumi	BE_13_01_05_00_Auglīgās augsnes segumi
BE_13_01_07_00	_13	_01	_07	_00	Zālājs	BE_13_01_07_00_Zālājs
BE_13_01_09_00	_13	_01	_09	_00	Segumu pamatnes	BE_13_01_09_00_Segumu pamatnes
BE_13_01_11_00	_13	_01	_11	_00	Segumi virs grunts	BE_13_01_11_00_Segumi virs grunts
BE_13_01_13_00	_13	_01	_13	_00	Segumi virs pārsegumiem	BE_13_01_13_00_Segumi virs pārsegumiem
BE_13_03_00_00	_13	_03	_00	_00	Seguma papildelementi	BE_13_03_00_00_Seguma papildelementi
BE_13_03_01_00	_13	_03	_01	_00	Apmales	BE_13_03_01_00_Apmales
BE_13_03_03_00	_13	_03	_03	_00	Segumu salaidumi	BE_13_03_03_00_Segumu salaidumi
BE_13_03_05_00	_13	_03	_05	_00	Ūdens novadīšanas elementi	BE_13_03_05_00_Ūdens novadīšanas elementi
BE_13_03_07_00	_13	_03	_07	_00	Citi seguma papildelementi	BE_13_03_07_00_Citi seguma papildelementi
BE_13_05_00_00	_13	_05	_00	_00	Aprīkojums	BE_13_05_00_00_Aprīkojums
BE_13_05_01_00	_13	_05	_01	_00	Atkritumu urnas	BE_13_05_01_00_Atkritumu urnas
BE_13_05_03_00	_13	_05	_03	_00	Velosipēdu statīvi	BE_13_05_03_00_Velosipēdu statīvi
BE_13_05_05_00	_13	_05	_05	_00	Soli	BE_13_05_05_00_Soli
BE_13_05_07_00	_13	_05	_07	_00	Stendi	BE_13_05_07_00_Stendi
BE_13_05_09_00	_13	_05	_09	_00	Masti	BE_13_05_09_00_Masti
BE_13_05_11_00	_13	_05	_11	_00	Sporta laukumi	BE_13_05_11_00_Sporta laukumi
BE_13_05_13_00	_13	_05	_13	_00	Bērnu rotaļu laukums	BE_13_05_13_00_Bērnu rotaļu laukums
BE_13_05_15_00	_13	_05	_15	_00	Cits aprīkojums	BE_13_05_15_00_Cits aprīkojums
BE_13_07_00_00	_13	_07	_00	_00	Nožogojums	BE_13_07_00_00_Nožogojums
BE_13_07_01_00	_13	_07	_01	_00	Žogs	BE_13_07_01_00_Žogs
BE_13_07_03_00	_13	_07	_03	_00	Vārti	BE_13_07_03_00_Vārti
BE_13_07_05_00	_13	_07	_05	_00	Automātiski paceļamā barjera	BE_13_07_05_00_Automātiski paceļamā barjera
BE_13_07_07_00	_13	_07	_07	_00	Atbalstsienas	BE_13_07_07_00_Atbalstsienas
BE_13_09_00_00	_13	_09	_00	_00	Āra kāpnes un pandusi	BE_13_09_00_00_Āra kāpnes un pandusi
BE_13_09_01_00	_13	_09	_01	_00	Kāpnes	BE_13_09_01_00_Kāpnes
BE_13_09_03_00	_13	_09	_03	_00	Pandusi	BE_13_09_03_00_Pandusi
BE_13_11_00_00	_13	_11	_00	_00	Apstādījumi	BE_13_11_00_00_Apstādījumi
BE_13_11_01_00	_13	_11	_01	_00	Koki	BE_13_11_01_00_Koki
BE_13_11_03_00	_13	_11	_03	_00	Krūmi	BE_13_11_03_00_Krūmi
BE_13_11_05_00	_13	_11	_05	_00	Augi	BE_13_11_05_00_Augi
BE_13_11_07_00	_13	_11	_07	_00	Citi apstādījumi	BE_13_11_07_00_Citi apstādījumi
BE_15_00_00_00	_15	_00	_00	_00	Teritorijas ceļi un laukumi (TS-CD)	BE_15_00_00_00_Teritorijas ceļi un laukumi (TS-CD)
BE_15_01_00_00	_15	_01	_00	_00	Segumi	BE_15_01_00_00_Segumi
BE_15_01_01_00	_15	_01	_01	_00	Asfaltbetona segumi	BE_15_01_01_00_Asfaltbetona segumi
BE_15_01_03_00	_15	_01	_03	_00	Betona segumi	BE_15_01_03_00_Betona segumi
BE_15_01_05_00	_15	_01	_05	_00	Bruģakmens segumi	BE_15_01_05_00_Bruģakmens segumi
BE_15_01_07_00	_15	_01	_07	_00	Zālājs	BE_15_01_07_00_Zālājs
BE_15_01_09_00	_15	_01	_09	_00	Stiprināts zālājs	BE_15_01_09_00_Stiprināts zālājs
BE_15_01_11_00	_15	_01	_11	_00	Citi segumi	BE_15_01_11_00_Citi segumi
BE_15_03_00_00	_15	_03	_00	_00	Satiksmes organizēšanas elementi	BE_15_03_00_00_Satiksmes organizēšanas elementi
BE_15_03_01_00	_15	_03	_01	_00	Ceļa zīmes	BE_15_03_01_00_Ceļa zīmes
BE_15_03_03_00	_15	_03	_03	_00	Horizontālie apzīmējumi	BE_15_03_03_00_Horizontālie apzīmējumi
BE_15_03_05_00	_15	_03	_05	_00	Luksofori	BE_15_03_05_00_Luksofori
BE_15_05_00_00	_15	_05	_00	_00	Seguma papildelementi	BE_15_05_00_00_Seguma papildelementi
BE_15_05_01_00	_15	_05	_01	_00	Apmales	BE_15_05_01_00_Apmales
BE_15_05_03_00	_15	_05	_03	_00	Segumu salaidumi	BE_15_05_03_00_Segumu salaidumi
BE_15_05_05_00	_15	_05	_05	_00	Ūdens novadīšanas elementi	BE_15_05_05_00_Ūdens novadīšanas elementi
BE_15_05_07_00	_15	_05	_07	_00	Citi seguma papildelementi	BE_15_05_07_00_Citi seguma papildelementi
BE_15_07_00_00	_15	_07	_00	_00	Norobežojošie elementi	BE_15_07_00_00_Norobežojošie elementi
BE_15_07_01_00	_15	_07	_01	_00	Barjeras	BE_15_07_01_00_Barjeras
BE_15_07_03_00	_15	_07	_03	_00	Stabi	BE_15_07_03_00_Stabi
BE_17_00_00_00	_17	_00	_00	_00	Gaisa vadu sistēmas	BE_17_00_00_00_Gaisa vadu sistēmas
BE_17_01_00_00	_17	_01	_00	_00	Dūmu nosūce	BE_17_01_00_00_Dūmu nosūce
BE_17_01_01_00	_17	_01	_01	_00	Dūmu un karstuma izvades sistēma	BE_17_01_01_00_Dūmu un karstuma izvades sistēma
BE_17_01_03_00	_17	_01	_03	_00	Gaisa virsspiediena sistēmu	BE_17_01_03_00_Gaisa virsspiediena sistēmu
BE_17_01_05_00	_17	_01	_05	_00	Citas dūmu nosūces sistēmas	BE_17_01_05_00_Citas dūmu nosūces sistēmas
BE_17_03_00_00	_17	_03	_00	_00	Ventilācijas sistēmas	BE_17_03_00_00_Ventilācijas sistēmas
BE_17_03_01_00	_17	_03	_01	_00	Telpu ventilācijas sistēmas	BE_17_03_01_00_Telpu ventilācijas sistēmas
BE_17_03_01_01	_17	_03	_01	_01	Centralizētā mehāniskā ventilācijas sistēma	BE_17_03_01_01_Centralizētā mehāniskā ventilācijas sistēma
BE_17_03_01_03	_17	_03	_01	_03	Dabīgā ventilācijas sistēma	BE_17_03_01_03_Dabīgā ventilācijas sistēma
BE_17_03_01_05	_17	_03	_01	_05	Mehāniskā nosūces sistēma	BE_17_03_01_05_Mehāniskā nosūces sistēma
BE_17_03_01_07	_17	_03	_01	_07	Mehāniskā pieplūdes sistēma	BE_17_03_01_07_Mehāniskā pieplūdes sistēma
BE_17_03_01_09	_17	_03	_01	_09	Sanitārā mezgla ventilācijas sistēma	BE_17_03_01_09_Sanitārā mezgla ventilācijas sistēma
BE_17_03_01_11	_17	_03	_01	_11	Vietējā nosūces sistēma	BE_17_03_01_11_Vietējā nosūces sistēma
BE_17_03_01_13	_17	_03	_01	_13	Virtuves ventilācijas sistēma	BE_17_03_01_13_Virtuves ventilācijas sistēma
BE_17_03_01_15	_17	_03	_01	_15	Citas telpu ventilācijas sistēmas	BE_17_03_01_15_Citas telpu ventilācijas sistēmas
BE_17_03_03_00	_17	_03	_03	_00	Tvaiku nosūces sistēmas	BE_17_03_03_00_Tvaiku nosūces sistēmas
BE_17_03_05_00	_17	_03	_05	_00	Industriālās tvaiku nosūces sistēmas	BE_17_03_05_00_Industriālās tvaiku nosūces sistēmas
BE_17_03_07_00	_17	_03	_07	_00	Autostāvvietu ventilācijas sistēma	BE_17_03_07_00_Autostāvvietu ventilācijas sistēma
BE_17_03_09_00	_17	_03	_09	_00	Citas ventilācijas sistēmas	BE_17_03_09_00_Citas ventilācijas sistēmas
BE_17_05_00_00	_17	_05	_00	_00	Citas gaisa vadu sistēmas	BE_17_05_00_00_Citas gaisa vadu sistēmas
BE_19_00_00_00	_19	_00	_00	_00	Cauruļvadu sistēmas	BE_19_00_00_00_Cauruļvadu sistēmas
BE_19_01_00_00	_19	_01	_00	_00	Apkures sistēmas	BE_19_01_00_00_Apkures sistēmas
BE_19_01_01_00	_19	_01	_01	_00	Apkures sistēmas	BE_19_01_01_00_Apkures sistēmas
BE_19_01_03_00	_19	_01	_03	_00	Zemgrīdas apkures sistēmas	BE_19_01_03_00_Zemgrīdas apkures sistēmas
BE_19_01_05_00	_19	_01	_05	_00	Sniega un ledus kausēšanas sistēmas	BE_19_01_05_00_Sniega un ledus kausēšanas sistēmas
BE_19_01_07_00	_19	_01	_07	_00	Siltumsūkņu sistēmas	BE_19_01_07_00_Siltumsūkņu sistēmas
BE_19_01_09_00	_19	_01	_09	_00	Citas apkures sistēmas	BE_19_01_09_00_Citas apkures sistēmas
BE_19_03_00_00	_19	_03	_00	_00	Klimata kontroles sistēmas	BE_19_03_00_00_Klimata kontroles sistēmas
BE_19_03_01_00	_19	_03	_01	_00	Centralizētā kondicionēšanas sistēma	BE_19_03_01_00_Centralizētā kondicionēšanas sistēma
BE_19_03_03_00	_19	_03	_03	_00	Lokālā kondicionēšaas sistēma	BE_19_03_03_00_Lokālā kondicionēšaas sistēma
BE_19_03_05_00	_19	_03	_05	_00	Citas klimata kontroles sistēmas	BE_19_03_05_00_Citas klimata kontroles sistēmas
BE_19_05_00_00	_19	_05	_00	_00	Aukstumapgādes sistēmas	BE_19_05_00_00_Aukstumapgādes sistēmas
BE_19_05_01_00	_19	_05	_01	_00	Aukstumapgādes sistēmas	BE_19_05_01_00_Aukstumapgādes sistēmas
BE_19_05_03_00	_19	_05	_03	_00	Aukstumkameru sistēmas	BE_19_05_03_00_Aukstumkameru sistēmas
BE_19_05_05_00	_19	_05	_05	_00	Aukstumnoliktavu sistēmas	BE_19_05_05_00_Aukstumnoliktavu sistēmas
BE_19_05_07_00	_19	_05	_07	_00	Citas aukstumapgādes sistēmas	BE_19_05_07_00_Citas aukstumapgādes sistēmas
BE_19_07_00_00	_19	_07	_00	_00	Ūdensapgādes sistēmas - iekšējās	BE_19_07_00_00_Ūdensapgādes sistēmas - iekšējās
BE_19_07_01_00	_19	_07	_01	_00	Aukstā ūdens apgādes sistēmas	BE_19_07_01_00_Aukstā ūdens apgādes sistēmas
BE_19_07_03_00	_19	_07	_03	_00	Karstā ūdens apgādes sistēmas	BE_19_07_03_00_Karstā ūdens apgādes sistēmas
BE_19_07_05_00	_19	_07	_05	_00	Ūdens sagatavošanas sistēmas	BE_19_07_05_00_Ūdens sagatavošanas sistēmas
BE_19_07_07_00	_19	_07	_07	_00	Laistīšanas sistēmas	BE_19_07_07_00_Laistīšanas sistēmas
BE_19_07_09_00	_19	_07	_09	_00	Dekoratīvo ūdens objektu sistēmas	BE_19_07_09_00_Dekoratīvo ūdens objektu sistēmas
BE_19_07_11_00	_19	_07	_11	_00	Citas ūdensapgādes sistēmas	BE_19_07_11_00_Citas ūdensapgādes sistēmas
BE_19_09_00_00	_19	_09	_00	_00	Kanalizācijas sistēmas - iekšējās	BE_19_09_00_00_Kanalizācijas sistēmas - iekšējās
BE_19_09_01_00	_19	_09	_01	_00	Kondensāta kanalizācijas sistēmas	BE_19_09_01_00_Kondensāta kanalizācijas sistēmas
BE_19_09_03_00	_19	_09	_03	_00	Rūpnieciskās kanalizācijas sistēmas	BE_19_09_03_00_Rūpnieciskās kanalizācijas sistēmas
BE_19_09_05_00	_19	_09	_05	_00	Sadzīves kanalizācijas sistēmas	BE_19_09_05_00_Sadzīves kanalizācijas sistēmas
BE_19_09_07_00	_19	_09	_07	_00	Virszemes ūdens uztveršanas sistēmas	BE_19_09_07_00_Virszemes ūdens uztveršanas sistēmas
BE_19_09_07_01	_19	_09	_07	_01	Lietus ūdens kanalizācijas sistēmas	BE_19_09_07_01_Lietus ūdens kanalizācijas sistēmas
BE_19_09_07_03	_19	_09	_07	_03	Notekūdeņu drenāžas sistēmas	BE_19_09_07_03_Notekūdeņu drenāžas sistēmas
BE_19_09_09_00	_19	_09	_09	_00	Citas iekšējās kanalizācijas sistēmas	BE_19_09_09_00_Citas iekšējās kanalizācijas sistēmas
BE_19_11_00_00	_19	_11	_00	_00	Ugunsdzēsības sistēmas	BE_19_11_00_00_Ugunsdzēsības sistēmas
BE_19_11_01_00	_19	_11	_01	_00	Automātiskās ūdens ugunsdzēsības sistēmas	BE_19_11_01_00_Automātiskās ūdens ugunsdzēsības sistēmas
BE_19_11_03_00	_19	_11	_03	_00	Gāzes ugunsdzēsības sistēmas	BE_19_11_03_00_Gāzes ugunsdzēsības sistēmas
BE_19_11_05_00	_19	_11	_05	_00	Pulvera ugunsdzēsības sistēmas	BE_19_11_05_00_Pulvera ugunsdzēsības sistēmas
BE_19_11_07_00	_19	_11	_07	_00	Putu ugunsdzēsības sistēmas	BE_19_11_07_00_Putu ugunsdzēsības sistēmas
BE_19_11_09_00	_19	_11	_09	_00	Ugunsdzēsības pārvietojamais aprīkojums	BE_19_11_09_00_Ugunsdzēsības pārvietojamais aprīkojums
BE_19_11_11_00	_19	_11	_11	_00	Ūdens ugunsdzēsības sistēmas	BE_19_11_11_00_Ūdens ugunsdzēsības sistēmas
BE_19_11_13_00	_19	_11	_13	_00	Citas ugunsdzēsības sistēmas	BE_19_11_13_00_Citas ugunsdzēsības sistēmas
BE_19_13_00_00	_19	_13	_00	_00	Gāzes apgāde sistēmas	BE_19_13_00_00_Gāzes apgāde sistēmas
BE_19_13_01_00	_19	_13	_01	_00	Kurināmās gāzes apgāde-iekšējā	BE_19_13_01_00_Kurināmās gāzes apgāde-iekšējā
BE_19_13_01_01	_19	_13	_01	_01	Dabasgāzes iekšējo gāzesvadu sistēma	BE_19_13_01_01_Dabasgāzes iekšējo gāzesvadu sistēma
BE_19_13_01_03	_19	_13	_01	_03	Sašķidrinātās naftas gāzes iekšējo gāzesvadu sistēma	BE_19_13_01_03_Sašķidrinātās naftas gāzes iekšējo gāzesvadu sistēma
BE_19_13_01_05	_19	_13	_01	_05	Citas kurināmās gāzes apgādes sistēmas	BE_19_13_01_05_Citas kurināmās gāzes apgādes sistēmas
BE_19_13_03_00	_19	_13	_03	_00	Kurināmās gāzes apgāde-ārējā 	BE_19_13_03_00_Kurināmās gāzes apgāde-ārējā 
BE_19_13_03_01	_19	_13	_03	_01	Dabasgāzes ārējo gāzesvadu sistēma	BE_19_13_03_01_Dabasgāzes ārējo gāzesvadu sistēma
BE_19_13_03_03	_19	_13	_03	_03	Sašķidrinātās naftas gāzes ārējo gāzesvadu sistēma	BE_19_13_03_03_Sašķidrinātās naftas gāzes ārējo gāzesvadu sistēma
BE_19_13_03_05	_19	_13	_03	_05	Citas kurināmās gāzes apgādes sistēmas	BE_19_13_03_05_Citas kurināmās gāzes apgādes sistēmas
BE_19_13_05_00	_19	_13	_05	_00	Vakuuma sistēmas	BE_19_13_05_00_Vakuuma sistēmas
BE_19_13_07_00	_19	_13	_07	_00	Industriālās gāzu apgādes sistēmas	BE_19_13_07_00_Industriālās gāzu apgādes sistēmas
BE_19_13_09_00	_19	_13	_09	_00	Laboratorijas gāzu apgādes sistēmas	BE_19_13_09_00_Laboratorijas gāzu apgādes sistēmas
BE_19_13_11_00	_19	_13	_11	_00	Medicīnas gāzu apgādes sistēmas	BE_19_13_11_00_Medicīnas gāzu apgādes sistēmas
BE_19_13_13_00	_19	_13	_13	_00	Citas gāzes apgādes sistēmas	BE_19_13_13_00_Citas gāzes apgādes sistēmas
BE_19_15_00_00	_19	_15	_00	_00	Tvaika apgādes sistēmas	BE_19_15_00_00_Tvaika apgādes sistēmas
BE_19_17_00_00	_19	_17	_00	_00	Degvielas ieguves, apstrādes un uzglabāšanas sistēmas	BE_19_17_00_00_Degvielas ieguves, apstrādes un uzglabāšanas sistēmas
BE_19_19_00_00	_19	_19	_00	_00	Degvielas apgādes sistēmas	BE_19_19_00_00_Degvielas apgādes sistēmas
BE_19_21_00_00	_19	_21	_00	_00	Industriālo šķidrumu apgādes sistēmas	BE_19_21_00_00_Industriālo šķidrumu apgādes sistēmas
BE_19_23_00_00	_19	_23	_00	_00	Gāzes ieguves, apstrādes un uzglabāšanas sistēmas	BE_19_23_00_00_Gāzes ieguves, apstrādes un uzglabāšanas sistēmas
BE_19_25_00_00	_19	_25	_00	_00	Siltumapgādes sistēmas - ārējās	BE_19_25_00_00_Siltumapgādes sistēmas - ārējās
BE_19_25_01_00	_19	_25	_01	_00	Laukumu siltumapgādes sistēmas	BE_19_25_01_00_Laukumu siltumapgādes sistēmas
BE_19_25_03_00	_19	_25	_03	_00	Visparējās siltumapgādes sistēma	BE_19_25_03_00_Visparējās siltumapgādes sistēma
BE_19_25_05_00	_19	_25	_05	_00	Siltumsūkņu sistēmas	BE_19_25_05_00_Siltumsūkņu sistēmas
BE_19_25_07_00	_19	_25	_07	_00	Citas siltumapgādes sistēmas	BE_19_25_07_00_Citas siltumapgādes sistēmas
BE_19_27_00_00	_19	_27	_00	_00	Apūdeņošanas sistēmas	BE_19_27_00_00_Apūdeņošanas sistēmas
BE_19_29_00_00	_19	_29	_00	_00	Ūdensapgāde un kanalizācija-ārējā 	BE_19_29_00_00_Ūdensapgāde un kanalizācija-ārējā 
BE_19_29_01_00	_19	_29	_01	_00	Ūdens ieguves, apstrādes un uzglabāšanas sistēmas	BE_19_29_01_00_Ūdens ieguves, apstrādes un uzglabāšanas sistēmas
BE_19_29_01_01	_19	_29	_01	_01	Ūdens ieguves sistēmas	BE_19_29_01_01_Ūdens ieguves sistēmas
BE_19_29_01_03	_19	_29	_01	_03	Ūdens apstrādes sistēmas	BE_19_29_01_03_Ūdens apstrādes sistēmas
BE_19_29_01_05	_19	_29	_01	_05	Ūdens attīrīšanas sistēma	BE_19_29_01_05_Ūdens attīrīšanas sistēma
BE_19_29_01_07	_19	_29	_01	_07	Ūdens dezinfekcijas sistēmas	BE_19_29_01_07_Ūdens dezinfekcijas sistēmas
BE_19_29_03_00	_19	_29	_03	_00	Ūdens uzglabāšanas sistēma	BE_19_29_03_00_Ūdens uzglabāšanas sistēma
BE_19_29_05_00	_19	_29	_05	_00	Aukstā ūdens apgādes sistēmas	BE_19_29_05_00_Aukstā ūdens apgādes sistēmas
BE_19_29_07_00	_19	_29	_07	_00	Karstā ūdens apgādes sistēmas	BE_19_29_07_00_Karstā ūdens apgādes sistēmas
BE_19_29_09_00	_19	_29	_09	_00	Ūdens kvalitātes uzraudzības sistēmas	BE_19_29_09_00_Ūdens kvalitātes uzraudzības sistēmas
BE_19_29_11_00	_19	_29	_11	_00	Laistīšanas sistēmas	BE_19_29_11_00_Laistīšanas sistēmas
BE_19_29_13_00	_19	_29	_13	_00	Dekoratīvo ūdens objektu sistēmas	BE_19_29_13_00_Dekoratīvo ūdens objektu sistēmas
BE_19_29_15_00	_19	_29	_15	_00	Citas ārējās ūdensapgādes sistēmas	BE_19_29_15_00_Citas ārējās ūdensapgādes sistēmas
BE_19_31_00_00	_19	_31	_00	_00	Ūdensapgāde un kanalizācija-ārējā 	BE_19_31_00_00_Ūdensapgāde un kanalizācija-ārējā 
BE_19_31_01_00	_19	_31	_01	_00	Ārējās notekūdeņu un kanalizācijas sistēma	BE_19_31_01_00_Ārējās notekūdeņu un kanalizācijas sistēma
BE_19_31_03_00	_19	_31	_03	_00	Revīzijas aku sistēmas	BE_19_31_03_00_Revīzijas aku sistēmas
BE_19_31_05_00	_19	_31	_05	_00	Zem zemes ūdens uztveršanas sistēmas	BE_19_31_05_00_Zem zemes ūdens uztveršanas sistēmas
BE_19_31_07_00	_19	_31	_07	_00	Zemes drenāžas sistēmas	BE_19_31_07_00_Zemes drenāžas sistēmas
BE_19_31_09_00	_19	_31	_09	_00	Notekūdeņu sistēmas	BE_19_31_09_00_Notekūdeņu sistēmas
BE_19_31_11_00	_19	_31	_11	_00	Notekūdeņu uzglabāšanas un attīrīšanas sistēma	BE_19_31_11_00_Notekūdeņu uzglabāšanas un attīrīšanas sistēma
BE_19_31_13_00	_19	_31	_13	_00	Notekūdeņu filtrācijas sistēmas	BE_19_31_13_00_Notekūdeņu filtrācijas sistēmas
BE_19_31_15_00	_19	_31	_15	_00	Notekūdeņu nosēdināšanas sistēmas	BE_19_31_15_00_Notekūdeņu nosēdināšanas sistēmas
BE_19_31_17_00	_19	_31	_17	_00	Smaku kontroles sistēma	BE_19_31_17_00_Smaku kontroles sistēma
BE_19_31_19_00	_19	_31	_19	_00	Aerācijas sistēmas	BE_19_31_19_00_Aerācijas sistēmas
BE_19_31_21_00	_19	_31	_21	_00	Nostādināšanas sistēmas	BE_19_31_21_00_Nostādināšanas sistēmas
BE_19_31_23_00	_19	_31	_23	_00	Lietus ūdens kanalizācijas tīkli 	BE_19_31_23_00_Lietus ūdens kanalizācijas tīkli 
BE_19_31_25_00	_19	_31	_25	_00	Citas cauruļvadu sistēmas	BE_19_31_25_00_Citas cauruļvadu sistēmas
BE_19_31_27_00	_19	_31	_27	_00	Kabeļu sistēmas	BE_19_31_27_00_Kabeļu sistēmas
BE_19_31_29_00	_19	_31	_29	_00	Nostādināšanas sistēmas	BE_19_31_29_00_Nostādināšanas sistēmas
BE_19_33_00_00	_19	_33	_00	_00	Lietus ūdens kanalizācijas tīkli 	BE_19_33_00_00_Lietus ūdens kanalizācijas tīkli 
BE_19_35_00_00	_19	_35	_00	_00	Citas cauruļvadu sistēmas	BE_19_35_00_00_Citas cauruļvadu sistēmas
BE_21_00_00_00	_21	_00	_00	_00	Kabeļu sistēmas	BE_21_00_00_00_Kabeļu sistēmas
BE_21_01_00_00	_21	_01	_00	_00	Iekšējās elektroapgādes sistēmas	BE_21_01_00_00_Iekšējās elektroapgādes sistēmas
BE_21_01_01_00	_21	_01	_01	_00	Apgaismojuma sistēmas	BE_21_01_01_00_Apgaismojuma sistēmas
BE_21_01_03_00	_21	_01	_03	_00	Autouzlādes sistēmas	BE_21_01_03_00_Autouzlādes sistēmas
BE_21_01_05_00	_21	_01	_05	_00	Avārijas apgaismojuma sistēmas	BE_21_01_05_00_Avārijas apgaismojuma sistēmas
BE_21_01_07_00	_21	_01	_07	_00	Elektroenerģijas ražošanas sistēmas	BE_21_01_07_00_Elektroenerģijas ražošanas sistēmas
BE_21_01_09_00	_21	_01	_09	_00	Pārvades tīklu sistēmas	BE_21_01_09_00_Pārvades tīklu sistēmas
BE_21_01_11_00	_21	_01	_11	_00	Zemējuma sistēmas	BE_21_01_11_00_Zemējuma sistēmas
BE_21_01_13_00	_21	_01	_13	_00	Zibensaizsardzības sistēmas	BE_21_01_13_00_Zibensaizsardzības sistēmas
BE_21_01_15_00	_21	_01	_15	_00	Citas iekšējās elektroapgādes sistēmas	BE_21_01_15_00_Citas iekšējās elektroapgādes sistēmas
BE_21_03_00_00	_21	_03	_00	_00	Ārējās elektroapgādes sistēmas	BE_21_03_00_00_Ārējās elektroapgādes sistēmas
BE_21_03_01_00	_21	_03	_01	_00	Apgaismojuma sistēmas	BE_21_03_01_00_Apgaismojuma sistēmas
BE_21_03_03_00	_21	_03	_03	_00	Autouzlādes sistēmas	BE_21_03_03_00_Autouzlādes sistēmas
BE_21_03_05_00	_21	_03	_05	_00	Elektroenerģijas ražošanas sistēmas	BE_21_03_05_00_Elektroenerģijas ražošanas sistēmas
BE_21_03_07_00	_21	_03	_07	_00	Pārvades tīklu sistēmas - augstsprieguma	BE_21_03_07_00_Pārvades tīklu sistēmas - augstsprieguma
BE_21_03_09_00	_21	_03	_09	_00	Pārvades tīklu sistēmas - zemsprieguma	BE_21_03_09_00_Pārvades tīklu sistēmas - zemsprieguma
BE_21_03_11_00	_21	_03	_11	_00	Zemējuma sistēmas	BE_21_03_11_00_Zemējuma sistēmas
BE_21_03_13_00	_21	_03	_13	_00	Zibensaizsardzības sistēmas	BE_21_03_13_00_Zibensaizsardzības sistēmas
BE_21_03_15_00	_21	_03	_15	_00	Citas ārējās elektroapgādes sistēmas	BE_21_03_15_00_Citas ārējās elektroapgādes sistēmas
BE_21_05_00_00	_21	_05	_00	_00	Elektronisko sakaru, drošības un kontroles sistēmas	BE_21_05_00_00_Elektronisko sakaru, drošības un kontroles sistēmas
BE_21_05_01_00	_21	_05	_01	_00	Elektronisko sakaru sistēmas	BE_21_05_01_00_Elektronisko sakaru sistēmas
BE_21_05_01_01	_21	_05	_01	_01	Centralizētās izziņošanas sistēmas	BE_21_05_01_01_Centralizētās izziņošanas sistēmas
BE_21_05_01_03	_21	_05	_01	_03	Elektronisko sakaru inženiertīkli sistēmas	BE_21_05_01_03_Elektronisko sakaru inženiertīkli sistēmas
BE_21_05_01_05	_21	_05	_01	_05	Sarunu iekārtu sistēmas	BE_21_05_01_05_Sarunu iekārtu sistēmas
BE_21_05_01_07	_21	_05	_01	_07	Citas elektronisko sakaru sistēmas	BE_21_05_01_07_Citas elektronisko sakaru sistēmas
BE_21_05_03_00	_21	_05	_03	_00	Apsardzes sistēmas	BE_21_05_03_00_Apsardzes sistēmas
BE_21_05_03_01	_21	_05	_03	_01	Apsardzes signalizācijas sistēmas	BE_21_05_03_01_Apsardzes signalizācijas sistēmas
BE_21_05_03_03	_21	_05	_03	_03	Drošības pārbaudes sistēmas	BE_21_05_03_03_Drošības pārbaudes sistēmas
BE_21_05_03_05	_21	_05	_03	_05	Novērošanas sistēmas	BE_21_05_03_05_Novērošanas sistēmas
BE_21_05_03_07	_21	_05	_03	_07	Piekļuves kontroles sistēmas	BE_21_05_03_07_Piekļuves kontroles sistēmas
BE_21_05_03_09	_21	_05	_03	_09	Citas apsardzes sistēmas	BE_21_05_03_09_Citas apsardzes sistēmas
BE_21_05_05_00	_21	_05	_05	_00	Drošības un apziņošanas sistēmas	BE_21_05_05_00_Drošības un apziņošanas sistēmas
BE_21_05_05_01	_21	_05	_05	_01	Gāzes noplūdes noteikšanas sistēmas	BE_21_05_05_01_Gāzes noplūdes noteikšanas sistēmas
BE_21_05_05_03	_21	_05	_05	_03	Izsaukumu signalizācijas sistēmas	BE_21_05_05_03_Izsaukumu signalizācijas sistēmas
BE_21_05_05_05	_21	_05	_05	_05	Šķidrumu noplūdes noteikšanas sistēmas	BE_21_05_05_05_Šķidrumu noplūdes noteikšanas sistēmas
BE_21_05_05_07	_21	_05	_05	_07	Citas drošības un apziņošanas sistēmas	BE_21_05_05_07_Citas drošības un apziņošanas sistēmas
BE_21_05_07_00	_21	_05	_07	_00	Vides uzraudzības sistēmas	BE_21_05_07_00_Vides uzraudzības sistēmas
BE_21_05_07_01	_21	_05	_07	_01	Civilās trauksmes apziņošanas sistēmas	BE_21_05_07_01_Civilās trauksmes apziņošanas sistēmas
BE_21_05_07_03	_21	_05	_07	_03	Gāzes noplūdes noteikšanas sistēmas	BE_21_05_07_03_Gāzes noplūdes noteikšanas sistēmas
BE_21_05_07_05	_21	_05	_07	_05	Meteoroloģiskās novērošanas sistēmas	BE_21_05_07_05_Meteoroloģiskās novērošanas sistēmas
BE_21_05_07_07	_21	_05	_07	_07	Piejaukumu noteikšanas sistēmas	BE_21_05_07_07_Piejaukumu noteikšanas sistēmas
BE_21_05_07_09	_21	_05	_07	_09	Citas vides uzraudzības sistēmas	BE_21_05_07_09_Citas vides uzraudzības sistēmas
BE_21_05_09_00	_21	_05	_09	_00	Vadības un automatizācijas sistēmas	BE_21_05_09_00_Vadības un automatizācijas sistēmas
BE_21_05_09_01	_21	_05	_09	_01	Iekārtu vadības un automatizācijas sistēmas	BE_21_05_09_01_Iekārtu vadības un automatizācijas sistēmas
BE_21_05_09_03	_21	_05	_09	_03	Konstrukciju uzraudzības sistēmas 	BE_21_05_09_03_Konstrukciju uzraudzības sistēmas 
BE_21_05_09_05	_21	_05	_09	_05	Skaitītāju un sensoru sistēmas	BE_21_05_09_05_Skaitītāju un sensoru sistēmas
BE_21_05_09_07	_21	_05	_09	_07	Transporta kustības vadības sistēmas	BE_21_05_09_07_Transporta kustības vadības sistēmas
BE_21_05_09_09	_21	_05	_09	_09	Citas vadības un automatizācijas sistēmas	BE_21_05_09_09_Citas vadības un automatizācijas sistēmas
BE_21_05_11_00	_21	_05	_11	_00	Reģistrācijas un biļešu pārvaldības sistēmas	BE_21_05_11_00_Reģistrācijas un biļešu pārvaldības sistēmas
BE_21_05_13_00	_21	_05	_13	_00	Citas elektronisko sakaru, drošības un kontroles sistēmas	BE_21_05_13_00_Citas elektronisko sakaru, drošības un kontroles sistēmas
BE_21_07_00_00	_21	_07	_00	_00	Ārējās elektronisko sakaru sistēmas	BE_21_07_00_00_Ārējās elektronisko sakaru sistēmas
BE_21_07_01_00	_21	_07	_01	_00	Elektronisko sakaru inženiertīklu sistēmas	BE_21_07_01_00_Elektronisko sakaru inženiertīklu sistēmas
BE_21_07_03_00	_21	_07	_03	_00	Signalizācijas sistēmas	BE_21_07_03_00_Signalizācijas sistēmas
BE_21_07_05_00	_21	_07	_05	_00	Vadības un automatizācijas sistēmas	BE_21_07_05_00_Vadības un automatizācijas sistēmas
BE_21_07_07_00	_21	_07	_07	_00	Citas ārējās elektronisko sakaru sistēmas	BE_21_07_07_00_Citas ārējās elektronisko sakaru sistēmas
BE_21_09_00_00	_21	_09	_00	_00	Ugunsdzēsības sistēmas	BE_21_09_00_00_Ugunsdzēsības sistēmas
BE_21_09_01_00	_21	_09	_01	_00	Ugunsdzēsības automātikas sistēmas 	BE_21_09_01_00_Ugunsdzēsības automātikas sistēmas 
BE_21_09_03_00	_21	_09	_03	_00	Ugunsgrēka atklāšanas un trauksmes signalizācijas sistēmas	BE_21_09_03_00_Ugunsgrēka atklāšanas un trauksmes signalizācijas sistēmas
BE_21_11_00_00	_21	_11	_00	_00	Citas kabeļu sistēmas	BE_21_11_00_00_Citas kabeļu sistēmas
BE_23_00_00	_23	_00	_00	_00	Tehnoloģija 	BE_23_00_00_Tehnoloģija 
BE_23_01_00	_23	_01	_00	_00	Tehnoloģiskais aprīkojums (TN)	BE_23_01_00_Tehnoloģiskais aprīkojums (TN)
BE_23_01_01	_23	_01	_01	_00	Transportēšanas sistēmas	BE_23_01_01_Transportēšanas sistēmas
BE_23_01_03	_23	_01	_03	_00	Stacionarās iekārtas	BE_23_01_03_Stacionarās iekārtas
BE_23_01_05	_23	_01	_05	_00	Cits tehnoloģiskais aprīkojums	BE_23_01_05_Cits tehnoloģiskais aprīkojums
Tabulas beigas						
 */
