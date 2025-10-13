using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;
using IFC.App.Bom.Models;

namespace IFC.App.Bom.Creators
{
    public class DefaultBomCreator : IBomCreator
    {
        private ElementStorage<DefaultElement> _elements = new ElementStorage<DefaultElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "ELEMENTI";
        public static readonly string Title = "ELEMENTU SPECIFIKĀCIJA";

        public DefaultBomCreator()
        {
            _sheetInfo= new SheetInfo
            {
                Title = Title,
                Headers = GetHeaders(),
                SheetName = WorksheetName
            };
        }

        // Keep existing methods unchanged
        public void PrintElements()
        {
            Console.WriteLine("=============================================================================================================");
            Console.WriteLine("==================================================ELEMENTI===================================================");
            Console.WriteLine("=============================================================================================================");
            var elements = _elements.GetElements();
            foreach (var element in elements)
            {
                element.Print();
            }
        }

        public void TakeElements(List<Assembly> assemblies)
        {
            List<Assembly> assembliesToRemove = new List<Assembly>();
            if (assemblies.Count > 0)
            {
                foreach (var assembly in assemblies)
                {
                    var defaultElement = new DefaultElement
                    {
                        Nosaukums = assembly.Name,
                        Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                        Tilpums = assembly.GetDoubleProp("VOLUME") / 1e9
                    };
                    _elements.Add(defaultElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi papildu elementi elementi!");
                return;
            }
            foreach (var assembly in assembliesToRemove)
            {
                assemblies.Remove(assembly);
            }
            _elements.SortByMark();
        }

        // Add new ExportToExcel method

        public List<IElement> GetElements()
        {
            throw new NotImplementedException();
        }

        public List<string> GetHeaders()
        {
            return ["MARKA", "NOSAUKUMS", "SKAITS", "TILPUMS"];
        }

        public List<string> GetPropertyNames()
        {
            throw new NotImplementedException();
        }

        public List<List<List<string>>> GetData()
        {
            //Get data like in @PrecastWallCreator.cs 
            var elements = _elements.GetElements();
            List<List<List<string>>> data = [];
            foreach (var element in elements)
            {
                List<string> row = new List<string>
                {
                    element.Marka.ToString(),
                    element.Nosaukums,
                    element.Skaits.ToString(),
                    Math.Round(element.Tilpums, 3).ToString()
                };
                data.Add([row]);
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
            return new List<List<string>>();
        }
    }
}
