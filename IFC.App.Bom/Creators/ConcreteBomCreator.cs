using IFC.App.Bom.Models;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    public class ConcreteBomCreator : IBomCreator
    {
        private ElementStorage<CipConcreteElement> _elements = new ElementStorage<CipConcreteElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "BETONA SPECIFIKĀCIJA";
        public static readonly string Title = "BETONA SPECIFIKĀCIJA";
        public ConcreteBomCreator()
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
                if (assembly.GetAssemblyType() == Assembly.AssemblyTypeEnum.IN_SITU_ASSEMBLY)
                {
                    var pt = assembly.GetMainPart() as Part;
                    if (pt == null) continue; // Skip if main part is not a Part
                    var concreteElement = CipConcreteElement.CreateFromAssembly(assembly);
                    _elements.Add(concreteElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi betona elementi.");
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
            Console.WriteLine("=======================================BETONA ELEMENTI=======================================================");
            Console.WriteLine("=============================================================================================================");
            var elements = _elements.GetElements();
            foreach (var element in elements)
            {
                element.Print();
            }
        }
        public SheetInfo GetSheetInfo()
        {
            return _sheetInfo;
        }
        public List<string> GetHeaders()
        {
            return new List<string>
            {
                "MARKA",
                "NOSAUKUMS",
                "SKAITS",
                "MATERIĀLS",
                "TILPUMS / m³",
                "STIEGROJUMS / kg",
            };
        }

        public List<IElement> GetElements()
        {
            return _elements.GetElements().Cast<IElement>().ToList();
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
                        Math.Round(element.Tilpums, round).ToString(),
                        Math.Round(element.Stiegrojums, round).ToString(),
                        ]]);
            }
            return data;
        }
        public int GetCount()
        {
            return _elements.GetCount();
        }
    }
}
