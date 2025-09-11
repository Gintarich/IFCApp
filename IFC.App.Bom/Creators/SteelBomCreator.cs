using IFC.App.Bom.Models;
using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;

namespace IFC.App.Bom.Creators
{
    public class SteelBomCreator : IBomCreator
    {
        private ElementStorage<SteelElement> _elements = new ElementStorage<SteelElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "TĒRAUDA SPECIFIKĀCIJA";
        public static readonly string Title = "TĒRAUDA SPECIFIKĀCIJA";

        public SteelBomCreator()
        {
            _sheetInfo = new SheetInfo
            {
                Title = Title,
                Headers = GetHeaders(),
                SheetName = WorksheetName
            };
        }

        public void TakeElements(List<Assembly> assemblies)
        {
            List<Assembly> assembliesToRemove = new List<Assembly>();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetAssemblyType() == Assembly.AssemblyTypeEnum.STEEL_ASSEMBLY)
                {
                    var matType = assembly.GetMainPart().GetStringProp("MATERIAL_TYPE");
                    if (matType != "STEEL") continue; //Filter out non-steel assemblies
                    var stElement = SteelElement.CreateFromAssembly(assembly);
                    _elements.Add(stElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi tērauda elementi.");
                return;
            }
            foreach (var assembly in assembliesToRemove)
            {
                assemblies.Remove(assembly);
            }
            _elements.SortByMark();
        }
        public void PrintElements()
        {
            Console.WriteLine("=============================================================================================================");
            Console.WriteLine("=======================================TĒRAUDA ELEMENTI=======================================================");
            Console.WriteLine("=============================================================================================================");
            var elements = _elements.GetElements();
            foreach (var element in elements)
            {
                element.Print();
            }
        }
        public List<IElement> GetElements()
        {
            return _elements.GetElements().Cast<IElement>().ToList();
        }
        public List<string> GetHeaders()
        {
            return new List<string>
            {
                "MARKA",
                "NOSAUKUMS",
                "SKAITS",
                "MATERIĀLS",
                "PROFILS",
                "GARUMS, mm",
                "SVARS, kg",
                "LAUKUMS, m²",
                "SVARS KOPĀ, kg",
                "LAUKUMS KOPĀ, m²",
            };
        }

        public List<List<List<string>>> GetData()
        {
            var round = 3;
            List<List<List<string>>> data = new List<List<List<string>>>();
            var elements = _elements.GetElements();
            foreach (var element in elements)
            {
                data.Add([[
                        element.Marka.ToString(),
                        element.Nosaukums,
                        element.Count.ToString(),
                        element.Materiāls,
                        element.Profils,
                        Math.Round(element.Garums, round).ToString(),
                        Math.Round(element.Svars, round).ToString(),
                        Math.Round(element.Laukums, round).ToString(),
                        Math.Round(element.SvarsKopā, round).ToString(),
                        Math.Round(element.LaukumsKopā, round).ToString(),
                        ]]);
            }
            return data;
        }

        public SheetInfo GetSheetInfo()
        {
            return _sheetInfo;
        }
        public int GetCount()
        {
            return _elements.GetCount();
        }
    }
}
