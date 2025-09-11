using ClosedXML.Excel;
using IFC.App.Bom.Creators;
using IFC.App.Bom.Models;

namespace IFC.App.Bom.Services
{
    public class ExcelService
    {
        private string _path;
        public ExcelService(string path)
        {
            _path = path;
        }

        public void ExportToExcel(List<IBomCreator> creators, PrintType thpe )
        {
            foreach (var creator in creators)
            {
                if(creator.GetCount() == 0) continue; // Skip if no elements to export
                if (thpe == PrintType.ToSingleFile)
                {
                    ExportToSingleFile(creator);
                }
                else if (thpe == PrintType.ToMultipleFiles)
                {
                    ExportToMultipleFiles(creator);
                }
                else
                {
                    throw new ArgumentException("Invalid print type specified.");
                }
            }        
        }
        public void ExportToSingleFile(IBomCreator creator)
        {
            XLWorkbook workbook;
            var info = creator.GetSheetInfo();
            if (File.Exists(_path))
            {
                workbook = new XLWorkbook(_path);
                // Remove worksheet if it already exists
                if (workbook.Worksheets.Contains(info.SheetName))
                {
                    workbook.Worksheet(info.SheetName).Delete();
                }
            }
            else
            {
                workbook = new XLWorkbook();
            }
            using (workbook)
            {
                IXLWorksheet ws = workbook.Worksheets.Add(info.SheetName);
                CreateHeaders(info, ws);
                CreateData(creator.GetData(), ws);

                try
                {
                    workbook.SaveAs(_path);
                }
                catch (IOException)
                {
                    throw new IOException("Unable to save the Excel file. Please ensure it is not open in another program.");
                }
                //open the file after saving
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _path,
                    UseShellExecute = true
                });
            }
        }
        public void ExportToMultipleFiles(IBomCreator creator)
        {

        }
        public void CreateHeaders(SheetInfo info, IXLWorksheet worksheet)
        {
            var columnCount = info.Headers.Count;
            var headers = info.Headers;
            var titleRange = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnCount));
            titleRange.Merge();
            titleRange.Value = info.Title;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 16;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //titleRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var headderRow = 2;
            for (int i = 0; i < headers.Count; i++)
            {
                 worksheet.Cell(headderRow, i + 1).Value = headers[i];
            }
            // Style headers
            var headerRow = worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, headers.Count));
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        public void CreateData(List<List<List<string>>> data, IXLWorksheet worksheet)
        {
            if (data.Count == 0 || data[0].Count == 0)  throw new ArgumentException("Data cannot be empty.");
            if (data[0].Count == 1)
            {
                CreateRegularData(data, worksheet);
            }
            else
            {
                CreateMultiLineData(data, worksheet);
            }
            worksheet.Columns().AdjustToContents();
        }

        private void CreateMultiLineData(List<List<List<string>>> data, IXLWorksheet worksheet)
        {
            int startingRow = 3;
            int currentRow = worksheet.LastRowUsed()?.RowNumber() + 1 ?? startingRow; // Start from row 3 or next available row
            foreach(var element in data)
            {
                if (element.Count == 0) continue; // Skip empty elements
                CreateDataRow(element[0], worksheet, currentRow); // Create main data row
                var dataRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, element[0].Count));
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                currentRow++;
                for (int i = 1; i < element.Count; i++)
                {
                    CreateDataRow(element[i], worksheet, currentRow); // Create additional rows for multi-line data

                    var layerRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, element[1].Count));
                    layerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    layerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    currentRow++;
                }
            }
        }

        private void CreateRegularData(List<List<List<string>>> list, IXLWorksheet worksheet)
        {
            int startingRow = 3;
            int currentRow = worksheet.LastRowUsed()?.RowNumber() + 1 ?? startingRow; // Start from row 3 or next available row
            foreach (var element in list)
            {
                CreateDataRow(element[0], worksheet, currentRow);
                currentRow++;
            }
            var dataRange = worksheet.Range(worksheet.Cell(startingRow, 1), worksheet.Cell(currentRow-1, list[0][0].Count));
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }
        private void CreateDataRow(List<string> data, IXLWorksheet worksheet, int row)
        {
            for (int i = 0; i < data.Count; i++)
            {
                // Check if data is integer or double and format accordingly
                if (int.TryParse(data[i], out int intValue))
                {
                    worksheet.Cell(row, i + 1).Value = intValue;
                }
                else if (double.TryParse(data[i], out double doubleValue))
                {
                    worksheet.Cell(row, i + 1).Value = Math.Round(doubleValue, 3);
                }
                else
                {
                    worksheet.Cell(row, i + 1).Value = data[i];
                }
            }
        }
    }

    public enum PrintType
    {
        ToSingleFile,
        ToMultipleFiles,
    }
}
