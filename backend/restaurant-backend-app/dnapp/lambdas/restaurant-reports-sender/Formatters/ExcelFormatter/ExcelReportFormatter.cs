using Function.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;

namespace Function.Formatters.ExcelFormatter
{
    public class ExcelReportFormatter : IReportFormatter
    {
        public string Format(List<SummaryEntry> summary)
        {
            // Ensure EPPlus license is accepted (required for non-commercial use)
            ExcelPackage.License.SetNonCommercialPersonal("My Name");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Weekly Report");

            // Add headers
            worksheet.Cells[1, 1].Value = "Location";
            worksheet.Cells[1, 2].Value = "Start Date";
            worksheet.Cells[1, 3].Value = "End Date";
            worksheet.Cells[1, 4].Value = "Waiter Name";
            worksheet.Cells[1, 5].Value = "Waiter Email";
            worksheet.Cells[1, 6].Value = "Current Hours";
            worksheet.Cells[1, 7].Value = "Previous Hours";
            worksheet.Cells[1, 8].Value = "Delta Hours";

            // Add data
            int row = 2;
            foreach (var entry in summary)
            {
                worksheet.Cells[row, 1].Value = entry.Location;
                worksheet.Cells[row, 2].Value = entry.StartDate;
                worksheet.Cells[row, 3].Value = entry.EndDate;
                worksheet.Cells[row, 4].Value = entry.WaiterName;
                worksheet.Cells[row, 5].Value = entry.WaiterEmail;
                worksheet.Cells[row, 6].Value = entry.CurrentHours;
                worksheet.Cells[row, 7].Value = entry.PreviousHours;
                worksheet.Cells[row, 8].Value = entry.DeltaHours;
                worksheet.Cells[row, 8].Style.Numberformat.Format = "+0.0%;-0.0%"; // Explicit + and - with %
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            // Convert to base64 string for email attachment
            var excelBytes = package.GetAsByteArray();
            return Convert.ToBase64String(excelBytes);
        }
    }
}
