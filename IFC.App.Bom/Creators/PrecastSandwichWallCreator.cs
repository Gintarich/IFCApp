using ClosedXML.Excel;
using IFC.App.Bom.Models;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    internal class PrecastSandwichWallCreator : IBomCreator
    {
        private ElementStorage<SandwichWallElement> _elements = new ElementStorage<SandwichWallElement>();
        private SheetInfo _sheetInfo;
        public static readonly string WorksheetName = "TRĪSSLĀŅU PANEĻI";
        public static readonly string Title = "TRĪSSLĀŅU PANEĻI";
        private readonly int _tWidth = 12;

        public PrecastSandwichWallCreator()
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
                if (assembly.GetAssemblyType() == Assembly.AssemblyTypeEnum.PRECAST_ASSEMBLY && assembly.Name == "TRĪSSLĀŅU SIENAS PANELIS")
                {
                    var swElement = SandwichWallElement.CreateFromAssembly(assembly);
                    swElement.MergeEqualLayers();
                    _elements.Add(swElement);
                    assembliesToRemove.Add(assembly);
                }
            }
            if (_elements.IsEmpty())
            {
                Console.WriteLine("Nav atrasti nekādi trīsslāņu paneļi.");
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

        //public void ExportToExcel(string filePath)
        //{
        //    int rowPointer = 0;
        //    var elements = _elements.GetElements();

        //    XLWorkbook workbook;
        //    if (File.Exists(filePath))
        //    {
        //        workbook = new XLWorkbook(filePath);
        //        // Remove worksheet if it already exists
        //        if (workbook.Worksheets.Contains(WorksheetName))
        //        {
        //            workbook.Worksheet(WorksheetName).Delete();
        //        }
        //    }
        //    else
        //    {
        //        workbook = new XLWorkbook();
        //    }

        //    using (workbook)
        //    {
        //        var worksheet = workbook.Worksheets.Add(WorksheetName);

        //        // Headers
        //        worksheet.Cell(2, 1).Value = "MARKA";
        //        worksheet.Cell(2, 2).Value = "NOSAUKUMS";
        //        worksheet.Cell(2, 3).Value = "SKAITS";
        //        worksheet.Cell(2, 4).Value = "MATERIĀLS";
        //        worksheet.Cell(2, 5).Value = "SLĀŅA BIEZUMS / mm";
        //        worksheet.Cell(2, 6).Value = "AUGSTUMS / mm";
        //        worksheet.Cell(2, 7).Value = "GARUMS / mm";
        //        worksheet.Cell(2, 8).Value = "TILPUMS ELEM. / m³";
        //        worksheet.Cell(2, 9).Value = "SVARS / t";
        //        worksheet.Cell(2, 10).Value = "TILPUMS KOPĀ / m³";
        //        worksheet.Cell(2, 11).Value = "BRUTO LAUKUMS KOPĀ / m²";
        //        worksheet.Cell(2, 12).Value = "NETO LAUKUMS KOPĀ / m²";

        //        // Style headers
        //        var headerRow = worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, _tWidth));
        //        headerRow.Style.Font.Bold = true;
        //        headerRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        //        ProcessData(worksheet);

        //        try
        //        {
        //            workbook.SaveAs(filePath);
        //        }
        //        catch (IOException)
        //        {
        //            throw new IOException("Unable to save the Excel file. Please ensure it is not open in another program.");
        //        }
        //    }
        //}

        //public void ProcessData(IXLWorksheet worksheet)
        //{
        //    int RowPointer = 3;
        //    var elements = _elements.GetElements();
        //    // Data
        //    for (int i = 0; i < elements.Count; i++)
        //    {
        //        var element = elements[i];
        //        AddAssemblyData(worksheet, element, RowPointer);
        //        RowPointer++;
        //        foreach (var layer in element.Layers)
        //        {
        //            AddPartData(worksheet, layer, RowPointer);
        //            RowPointer++;
        //        }
        //    }

        //    PrintSummary(worksheet, RowPointer);


        //    // Auto-fit columns
        //    worksheet.Columns().AdjustToContents();

        //}

        //private void AddAssemblyData(IXLWorksheet worksheet, SandwichWallElement element, int row)
        //{
        //    int round = 3;
        //    worksheet.Cell(row, 1).Value = element.Marka.ToString();
        //    worksheet.Cell(row, 2).Value = element.Nosaukums;
        //    worksheet.Cell(row, 3).Value = element.Count;
        //    worksheet.Cell(row, 4).Value = "";
        //    worksheet.Cell(row, 5).Value = Math.Round(element.Biezums, 0);
        //    worksheet.Cell(row, 6).Value = Math.Round(element.Augstums, 0);
        //    worksheet.Cell(row, 7).Value = Math.Round(element.Garums, 0);
        //    worksheet.Cell(row, 8).Value = Math.Round(element.Tilpums, round);
        //    worksheet.Cell(row, 9).Value = Math.Round(element.Svars, round);
        //    worksheet.Cell(row, 10).Value = Math.Round(element.TilpumsKopā, round);
        //    worksheet.Cell(row, 11).Value = Math.Round(element.BrutoLaukumsKopā, round);
        //    worksheet.Cell(row, 12).Value = Math.Round(element.NetoLaukumsKopā, round);

        //    var dataRange = worksheet.Range(worksheet.Cell(row, 1), worksheet.Cell(row, _tWidth));
        //    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //    dataRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        //}

        //private void AddPartData(IXLWorksheet worksheet, SandwichWallLayer element, int row)
        //{
        //    int round = 3;
        //    worksheet.Cell(row, 1).Value = "";
        //    worksheet.Cell(row, 2).Value = element.Nosaukums;
        //    worksheet.Cell(row, 3).Value = 1;
        //    worksheet.Cell(row, 4).Value = element.Materiāls;
        //    worksheet.Cell(row, 5).Value = Math.Round(element.Biezums, round);
        //    worksheet.Cell(row, 6).Value = Math.Round(element.Augstums, round);
        //    worksheet.Cell(row, 7).Value = Math.Round(element.Garums, 0);
        //    worksheet.Cell(row, 8).Value = Math.Round(element.Tilpums, round);
        //    worksheet.Cell(row, 9).Value = Math.Round(element.Weight, round);
        //    worksheet.Cell(row, 11).Value = Math.Round(element.BrutoLaukums, round);
        //    worksheet.Cell(row, 12).Value = Math.Round(element.NetoLaukums, round);

        //    var dataRange = worksheet.Range(worksheet.Cell(row, 1), worksheet.Cell(row, _tWidth));
        //    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //}

        //private void PrintSummary(IXLWorksheet worksheet, int currentRow)
        //{
        //    int summaryRow = currentRow + 2;
        //    worksheet.Cell(summaryRow, 1).Value = "KOPĀ";
        //    worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
        //    // Calculate totals
        //    worksheet.Cell(summaryRow, 10).FormulaA1 = $"=SUM(J3:J{summaryRow - 1})"; // Total Volume
        //    worksheet.Cell(summaryRow, 11).FormulaA1 = $"=SUM(K3:K{summaryRow - 1})"; // Total Gross Area
        //    worksheet.Cell(summaryRow, 12).FormulaA1 = $"=SUM(L3:L{summaryRow - 1})"; // Total Net Area
        //    var summaryRange = worksheet.Range(worksheet.Cell(summaryRow, 1), worksheet.Cell(summaryRow, _tWidth));
        //    summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //    summaryRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //    summaryRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        //}

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
            for (int i = 0; i < elements.Count; i++)
            {
                var el = elements[i];
                List<List<string>> layers = [];
                data.Add(layers);
                layers.Add([
                    el.Marka.ToString(),
                    el.Nosaukums,
                    el.Skaits.ToString(),
                    "",
                    Math.Round(el.Biezums, 0).ToString(),
                    Math.Round(el.Augstums, 0).ToString(),
                    Math.Round(el.Garums, 0).ToString(),
                    Math.Round(el.Tilpums, round).ToString(),
                    Math.Round(el.Svars, round).ToString(),
                    Math.Round(el.TilpumsKopā, round).ToString(),
                    Math.Round(el.BrutoLaukumsKopā, round).ToString(),
                    Math.Round(el.NetoLaukumsKopā, round).ToString()
                ]);
                foreach (var layer in el.Layers)
                {
                    layers.Add([
                        "",
                        layer.Nosaukums,
                        layer.Skaits.ToString(),
                        layer.Materiāls,
                        Math.Round(layer.Biezums, 0).ToString(),
                        Math.Round(layer.Augstums, 0).ToString(),
                        Math.Round(layer.Garums, 0).ToString(),
                        Math.Round(layer.Tilpums, round).ToString(),
                        Math.Round(layer.Weight, round).ToString(),
                        "",
                        Math.Round(layer.BrutoLaukums, round).ToString(),
                        Math.Round(layer.NetoLaukums, round).ToString()
                    ]);
                }
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
            Dictionary<string, List<double>> materialTotals = new Dictionary<string, List<double>>();

            foreach (var element in _elements.GetElements())
            {
                foreach (var layer in element.Layers)
                {
                    if (layer.Nosaukums == "SILTUMIZOLĀCIJA")
                    {
                        var name = $"{layer.Nosaukums} {Math.Round(layer.Biezums, 0)}mm";
                        var values = materialTotals.TryGetValue(name, out List<double> existingValues)
                            ? existingValues
                            : new List<double> { 0.0, 0.0, 0.0 };

                        values[0] += layer.Tilpums * element.Skaits; // Volume
                        values[1] += layer.BrutoLaukums * element.Skaits; // Area
                        values[2] += layer.NetoLaukums * element.Skaits; // Net Area

                        materialTotals[name] = values;

                    }
                    else if (layer.Nosaukums == "NESOŠAIS SLĀNIS")
                    {
                        var name = layer.Nosaukums;
                        var values = materialTotals.TryGetValue(name, out List<double> existingValues)
                            ? existingValues
                            : new List<double> { 0.0 , 0.0, 0.0};

                        values[0] += layer.Tilpums * element.Skaits; // Volume
                        values[1] += layer.BrutoLaukums * element.Skaits; // Area
                        values[2] += layer.NetoLaukums * element.Skaits; // Net Area

                        materialTotals[name] = values;
                    }
                    else if (layer.Nosaukums == "APDARES SLĀNIS")
                    {
                        var name = layer.Nosaukums;
                        var values = materialTotals.TryGetValue(name, out List<double> existingValues)
                            ? existingValues
                            : new List<double> { 0.0 , 0.0, 0.0};

                        values[0] += layer.Tilpums * element.Skaits; // Volume
                        values[1] += layer.BrutoLaukums * element.Skaits; // Area
                        values[2] += layer.NetoLaukums * element.Skaits; // Net Area

                        materialTotals[name] = values;
                    }
                }
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
            OutputData.Add(new List<string> {"", "SPECIFIKĀCIJĀ NAV UZRĀDĪTAS IEBETONĒJAMĀS DETAĻAS" });

            return OutputData;
        }
    }
}
