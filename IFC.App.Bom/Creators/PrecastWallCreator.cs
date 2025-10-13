using IFC.App.Bom.Models;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    public class PrecastWallCreator : IBomCreator
    {
        private ElementStorage<PrecastWallElement> _elements = new ElementStorage<PrecastWallElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "VIENSLĀŅA PANEĻI";
        public static readonly string Title = "VIENSLĀŅA PANEĻU SPECIFIKĀCIJA";
        private readonly int _tWidth = 12;

        public PrecastWallCreator()
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
                if (assembly.GetAssemblyType() == Assembly.AssemblyTypeEnum.PRECAST_ASSEMBLY && assembly.Name == "VIENSLĀŅU SIENAS PANELIS")
                {
                    var swElement = PrecastWallElement.CreateFromAssembly(assembly);
                    _elements.Add(swElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi vienslāņu paneļi.");
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
                        el.Skaits.ToString(),
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

        public void PrintHeader()
        {
            return;
        }

        public List<List<string>> GetSummaryData()
        {
            Dictionary<string, List<double>> materialTotals = new Dictionary<string, List<double>>();

            foreach (var element in _elements.GetElements())
            {
                var name = $"{element.Nosaukums} {Math.Round(element.Biezums,0)}";
                var values = materialTotals.TryGetValue(name, out List<double> existingValues)
                    ? existingValues
                    : new List<double> { 0.0, 0.0, 0.0 };

                values[0] += element.Tilpums * element.Skaits; // Volume
                values[1] += element.BrutoLaukums * element.Skaits; // Area
                values[2] += element.NetoLaukums * element.Skaits; // Net Area

                materialTotals[name] = values;

            }
            var OutputData = new List<List<string>>
            {
                new List<string>{"", "KOPĒJIE DATI PAR ELEMENTIEM"},
                new List<string>{"", "MATERIĀLS", "TILPUMS m³", "LAUKUMS BRUTO m²", "LAUKUMS NETO m²"}
            };

            foreach (var kvp in materialTotals)
            {
                OutputData.Add(new List<string> { "", kvp.Key,
                    Math.Round(kvp.Value[0], 3).ToString(),
                    Math.Round(kvp.Value[1], 3).ToString(),
                    Math.Round(kvp.Value[2], 3).ToString()
                });
            }

            OutputData.Add(new List<string> { "" });
            OutputData.Add(new List<string> { "", "SPECIFIKĀCIJĀ NAV UZRĀDĪTAS IEBETONĒJAMĀS DETAĻAS" });

            return OutputData;
        }
    }
}
