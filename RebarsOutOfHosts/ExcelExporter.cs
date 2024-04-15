using OfficeOpenXml;
using System.Windows;

namespace RebarsOutOfHosts
{

    internal class ExcelExporter
    {
        public void ExportTable(System.Data.DataTable table, string worksheetName, string fileName)
        {
            try
            {
                ExcelPackage excelTable = new ExcelPackage();
                ExcelWorksheet worksheet = excelTable.Workbook.Worksheets.Add(worksheetName);

                for (int i = 0; i <= table.Rows.Count; i++)
                    for (int j = 1; j <= table.Columns.Count; j++)
                        worksheet.Cells[i + 1, j].Value = i == 0 ? table.Columns[j - 1].ColumnName : table.Rows[i - 1].ItemArray[j - 1];

                using (ExcelRange excelRange = worksheet.Cells[1, 1, table.Rows.Count, table.Columns.Count])
                {
                    excelRange.AutoFitColumns();
                }
                try
                {
                    excelTable.SaveAs(new System.IO.FileInfo($"{fileName}.xlsx"));
                }
                catch (System.InvalidOperationException exception)
                {
                    if (exception.Message.Contains("Error saving file"))
                        MessageBox.Show("Не удалось выгрузить таблицу, так как таблица с таким же именем уже существует и открыта в другой программе. Закройте файл и повторите попытку",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch { }
        }
    }


}
