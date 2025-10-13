using IFC.App.Bom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    public class LintelBomCreator : IBomCreator
    {
        private ElementStorage<Lintel> _elements = new ElementStorage<Lintel>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "PĀRSEDZES";
        public static readonly string Title = "MŪRA PĀRSEDŽU SPECIFIKĀCIJA";
        private readonly int _tWidth = 12;

        public LintelBomCreator()
        {
            _sheetInfo = new SheetInfo
            {
                Title = Title,
                Headers = GetHeaders(),
                SheetName = WorksheetName
            };
        }

        public List<string> GetHeaders()
        {
            return new List<string>
            {
                 "MARKA",
                 "NOSAUKUMS",
                 "SKAITS",
                 "MATERIĀLS",
                 "BIEZUMS / mm",
                 "AUGSTUMS / mm",
                 "GARUMS / mm",
                 "TILPUMS ELEM. / m³",
                 "TILPUMS KOPĀ / m³",
            };
        }

        public int GetCount()
        {
            return _elements.GetCount();
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
                        Math.Round(el.TilpumsKopā, round).ToString(),
                        ]]);
            }
            return data;
        }

        public List<IElement> GetElements()
        {
            return _elements.GetElements().Cast<IElement>().ToList();
        }

        public SheetInfo GetSheetInfo()
        {
            return _sheetInfo;
        }

        public List<List<string>> GetSummaryData()
        {
            return new List<List<string>>();
        }

        public void TakeElements(List<Assembly> assemblies)
        {
            List<Assembly> assembliesToRemove = new List<Assembly>();
            foreach (var assembly in assemblies)
            {
                if (assembly.Name == "AILU PĀRSEDZE")
                {
                    var lintel = Lintel.CreateFromAssembly(assembly);
                    _elements.Add(lintel);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrastas mūra pārsedzes");
                return;
            }
            foreach (var assembly in assembliesToRemove)
            {
                assemblies.Remove(assembly);
            }
            _elements.SortByMark();
        }
    }
}
