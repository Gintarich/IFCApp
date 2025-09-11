using IFC.App.Bom.Models;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    internal class PrecastHcsCreator : IBomCreator
    {
        private ElementStorage<PrecastHcsElement> _elements = new ElementStorage<PrecastHcsElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "HCS";
        public static readonly string Title = "PĀRSEGUMA PANEĻU SPECIFIKĀCIJA";
        private readonly int _tWidth = 12;

        public PrecastHcsCreator()
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
                if (assembly.GetAssemblyType() == Assembly.AssemblyTypeEnum.PRECAST_ASSEMBLY && assembly.Name == "PĀRSEGUMA PANELIS")
                {
                    var swElement = PrecastHcsElement.CreateFromAssembly(assembly);
                    _elements.Add(swElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi pārseguma paneļi.");
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
                 "MATERIĀLS",
                 "SLĀŅA BIEZUMS / mm",
                 "AUGSTUMS / mm",
                 "GARUMS / mm",
                 "TILPUMS ELEM. / m³",
                 "SVARS / t",
                 "TILPUMS KOPĀ / m³",
                 "BRUTO LAUKUMS KOPĀ / m²",
                 "NETO LAUKUMS KOPĀ / m²",
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
                        el.Count.ToString(),
                        el.Materiāls,
                        Math.Round(el.Biezums, 0).ToString(),
                        Math.Round(el.Augstums, 0).ToString(),
                        Math.Round(el.Garums, 0).ToString(),
                        Math.Round(el.Tilpums, round).ToString(),
                        Math.Round(el.Svars, round).ToString(),
                        Math.Round(el.TilpumsKopā, round).ToString(),
                        Math.Round(el.BrutoLaukumsKopā, round).ToString(),
                        Math.Round(el.NetoLaukumsKopā, round).ToString()
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
