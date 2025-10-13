using IFC.App.Bom.Models;
using IFCApp.TeklaServices.Utils;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    public class PrecastBomCreator : IBomCreator
    {
        private ElementStorage<PrecastElement> _elements = new ElementStorage<PrecastElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "SALIEKAMAIS DZB";
        public static readonly string Title = "SALIEKAMĀ DZB SPECIFIKĀCIJA";

        public PrecastBomCreator()
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
                if (assembly.GetAssemblyType() == Assembly.AssemblyTypeEnum.PRECAST_ASSEMBLY && assembly.Name != "PABETONĒJUMS")
                {
                    var precastElement = new PrecastElement
                    {
                        Nosaukums = assembly.Name,
                        Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                        Tilpums = assembly.GetDoubleProp("VOLUME") / 1e9
                    };
                    _elements.Add(precastElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi saliekamie dzelsbetona papildu elementi.");
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
            Console.WriteLine("=======================================SALIEKAMĀ DZELZSBETONA ELEMENTI=======================================");
            Console.WriteLine("=============================================================================================================");

            var elements = _elements.GetElements();
            foreach (var element in elements)
            {
                element.Print();
            }
        }


        public List<IElement> GetElements()
        {
            throw new NotImplementedException();
        }

        public List<string> GetHeaders()
        {
            return new List<string>
            {
                 "MARKA",
                 "NOSAUKUMS",
                 "SKAITS",
                 "TILPUMS ELEM. / m³",
            };
        }

        public List<List<List<string>>> GetData()
        {
            var round = 3;
            List<List<List<string>>> data = [];
            var elements = _elements.GetElements();
            foreach (var el in elements)
            {
                data.Add([[
                        el.Marka.ToString(),
                        el.Nosaukums,
                        el.Skaits.ToString(),
                        Math.Round(el.TilpumsKopā, round).ToString(),
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

        public List<List<string>> GetSummaryData()
        {
            return new List<List<string>>
            {
            };
        }
    }
}
