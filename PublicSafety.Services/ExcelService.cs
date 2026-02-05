using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class ExcelService
    {

        public static ExcelUploadResult AddEmployeesFromExcel(Stream fileStream)
        {
            List<string> errors = new List<string>();
            List<Employee> employees = new List<Employee>();
            List<EmployeeJobTitleHistory> histories = new List<EmployeeJobTitleHistory>();

            using (var wb = new XLWorkbook(fileStream))
            {
                var ws = wb.Worksheet(1);
                var rows = ws.RangeUsed().RowsUsed();

                foreach (var row in rows.Skip(1)) // ✅ Skip header
                {
                    try
                    {
                        // ✅ Column A: Employee Number
                        string employeeNumber = row.Cell(1).GetString().Trim();

                        // ✅ Duplicate check
                        if (EmployeeRepo.EmployeeNumberExists(employeeNumber))
                        {
                            errors.Add($"Row {row.RowNumber()}: الرقم الوظيفي ({employeeNumber}) مستخدم مسبقاً.");
                            continue;
                        }

                        // ✅ Column B–D: Names
                        string firstName = row.Cell(2).GetString().Trim();
                        string secondName = row.Cell(3).GetString().Trim();
                        string lastName = row.Cell(4).GetString().Trim();

                        // ✅ Column E–F: Contact
                        string email = row.Cell(5).GetString().Trim();
                        string phone = row.Cell(6).GetString().Trim();

                        // ✅ Column G: Employment Date
                        DateTime employmentDate = row.Cell(7).GetDateTime();

                        // ✅ Column H: Work Location
                        string workLocationStr = row.Cell(8).GetString().Trim();

                        if (!Enum.TryParse(workLocationStr, true, out enWorkLocation workLocation))
                        {
                            errors.Add($"Row {row.RowNumber()}: موقع العمل غير صحيح ({workLocationStr}).");
                            continue;
                        }

                        // ✅ Column I–K: Department / Section / JobTitle
                        string departmentName = row.Cell(9).GetString().Trim();
                        string sectionName = row.Cell(10).GetString().Trim();
                        string jobTitleName = row.Cell(11).GetString().Trim();

                        var dept = DepartmentService.GetDepartmentByName(departmentName);
                        if (dept == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: الوحدة التنظيمية غير صحيحة ({departmentName}).");
                            continue;
                        }

                        var section = SectionService.GetSectionByName(sectionName);
                        if (section == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: القسم غير صحيح ({sectionName}).");
                            continue;
                        }

                        var jobTitle = JobTitleService.GetJobTitleByName(jobTitleName);
                        if (jobTitle == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: المسمى الوظيفي غير صحيح ({jobTitleName}).");
                            continue;
                        }

                        // ✅ Create Employee
                        var employee = new Employee
                        {
                            EmployeeId = Guid.NewGuid(),
                            EmployeeNumber = employeeNumber,

                            FirstName = firstName,
                            SecondName = secondName,
                            LastName = lastName,
                            FullName = firstName + " " + secondName + " " + lastName,

                            Email = email,
                            Phone = phone,

                            EmploymentDate = employmentDate,
                            WorkLocation = workLocation,

                            DepartmentId = dept.DepartmentId,
                            SectionId = section.SectionId,
                            JobTitleId = jobTitle.JobTitleId,

                            Active = true,
                            IsIntern = true,

                            CreationDate = DateTime.Now,
                            JobTitleUpdateDate = employmentDate
                        };

                        employees.Add(employee);

                        // ✅ Add JobTitle History like AddNewEmployee()
                        histories.Add(new EmployeeJobTitleHistory
                        {
                            EmployeeJobTitleHistoryId = Guid.NewGuid(),
                            EmployeeId = employee.EmployeeId,
                            JobTitleId = employee.JobTitleId,
                            StartDate = employmentDate,
                            EndDate = null
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row.RowNumber()}: {ex.Message}");
                    }
                }
            }

            // ✅ Save Only if we have employees
            if (employees.Any())
            {
                EmployeeRepo.AddRange(employees);
                EmployeeRepo.AddJobTitleHistoryRange(histories);
            }

            return new ExcelUploadResult
            {
                SuccessCount = employees.Count,
                Errors = errors
            };
        }



        public static byte[] GenerateEmployeeTemplate()
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("EmployeesTemplate");

                // ✅ Headers (Required Only)
                ws.Cell("A1").Value = "Employee Number (رقم وظيفي)*";
                ws.Cell("B1").Value = "First Name (الاسم الأول)*";
                ws.Cell("C1").Value = "Second Name (اسم الأب)*";
                ws.Cell("D1").Value = "Last Name (اسم العائلة)*";

                ws.Cell("E1").Value = "Email (البريد الإلكتروني)";
                ws.Cell("F1").Value = "Phone (رقم الهاتف)";

                ws.Cell("G1").Value = "Employment Date (yyyy-mm-dd)*";
                ws.Cell("H1").Value = "Work Location (Amman/Khaldieh)*";

                ws.Cell("I1").Value = "Department Name (الوحدة التنظيمية)*";
                ws.Cell("J1").Value = "Section Name (القسم)*";
                ws.Cell("K1").Value = "Job Title (المسمى الوظيفي)*";

                // ✅ Style Header Row
                var headerRange = ws.Range("A1:K1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // ✅ Column Widths
                ws.Column(1).Width = 22;  // Employee Number
                ws.Column(2).Width = 18;  // First Name
                ws.Column(3).Width = 18;  // Second Name
                ws.Column(4).Width = 18;  // Last Name
                ws.Column(5).Width = 25;  // Email
                ws.Column(6).Width = 18;  // Phone
                ws.Column(7).Width = 28;  // Employment Date
                ws.Column(8).Width = 28;  // Work Location
                ws.Column(9).Width = 25;  // Department
                ws.Column(10).Width = 25; // Section
                ws.Column(11).Width = 25; // Job Title

                // ✅ Example Row (Optional)
                ws.Cell("A2").Value = "1023";
                ws.Cell("B2").Value = "Ahmad";
                ws.Cell("C2").Value = "Mohammad";
                ws.Cell("D2").Value = "Saleh";
                ws.Cell("E2").Value = "ahmad@example.com";
                ws.Cell("F2").Value = "0790000000";
                ws.Cell("G2").Value = "2026-01-15";
                ws.Cell("H2").Value = "Amman";
                ws.Cell("I2").Value = "IT Department";
                ws.Cell("J2").Value = "Development Section";
                ws.Cell("K2").Value = "Software Engineer";

                ws.Range("A2:K2").Style.Font.FontColor = XLColor.DarkGray;

                // ✅ Footer Note
                ws.Cell("A4").Value =
                    "ملاحظة: الحقول التي تحتوي على علامة * إلزامية.";
                ws.Range("A4:K4").Merge().Style.Font.Italic = true;

                // ✅ Export File
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }


        public static byte[] ExportAllEmployeesToExcel()
        {
            var employees = EmployeeRepo.GetAllActiveEmployees(); // fetch all employees

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Employees");

                // Header row
                ws.Cell(1, 1).Value = "First Name";
                ws.Cell(1, 2).Value = "Second Name";
                ws.Cell(1, 3).Value = "Last Name";
                ws.Cell(1, 4).Value = "Email";
                ws.Cell(1, 5).Value = "Phone";
                ws.Cell(1, 6).Value = "Employment Date";
                ws.Cell(1, 7).Value = "Work Location";
                ws.Cell(1, 8).Value = "Department";
                ws.Cell(1, 9).Value = "Section";
                ws.Cell(1, 10).Value = "Job Title";

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 30;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 20;
                ws.Column(10).Width = 20;

                int row = 2;
                foreach (var emp in employees)
                {
                    ws.Cell(row, 1).Value = emp.FirstName;
                    ws.Cell(row, 2).Value = emp.SecondName;
                    ws.Cell(row, 3).Value = emp.LastName;
                    ws.Cell(row, 4).Value = emp.Email;
                    ws.Cell(row, 5).Value = emp.Phone;
                    ws.Cell(row, 6).Value = emp.EmploymentDate.ToString("yyyy-MM-dd");
                    ws.Cell(row, 7).Value = emp.WorkLocation.ToString();
                    ws.Cell(row, 8).Value = emp.Department?.Name;
                    ws.Cell(row, 9).Value = emp.Section?.Name;
                    ws.Cell(row, 10).Value = emp.JobTitle?.Name;
                    row++;
                }

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }


        public static byte[] ExportEmployeeEntitlements(
         List<EmployeeEntitlementExportRow> data,
         int year)
        {
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add($"استحقاقات {year}");

                // ✅ RTL Mode
                sheet.RightToLeft = true;

                // ✅ Arabic alignment
                sheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // ✅ Headers
        

                // ✅ Arabic headers (rightmost first)
                sheet.Cell(1, 9).Value = "الرقم الوظيفي";
                sheet.Cell(1, 8).Value = "اسم الموظف";
                sheet.Cell(1, 7).Value = "الوحدة التنظيمية";
                sheet.Cell(1, 6).Value = "القسم";
                sheet.Cell(1, 5).Value = "التصنيف";
                sheet.Cell(1, 4).Value = "اسم المادة";
                sheet.Cell(1, 3).Value = "المستحق";
                sheet.Cell(1, 2).Value = "المصروف";
                sheet.Cell(1, 1).Value = "مستلم؟";
                // ✅ Header style
                var headerRange = sheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                // ✅ Sort
                var sortedData = data
                    .OrderBy(x => x.Department)
                    .ThenBy(x => x.Section)
                    .ThenBy(x => x.EmployeeName)
                    .ThenBy(x => x.ItemName)
                    .ToList();

                // ✅ Fill rows
                int row = 2;

                foreach (var item in sortedData)
                {
                    sheet.Cell(row, 9).Value = item.EmployeeNumber;
                    sheet.Cell(row, 8).Value = item.EmployeeName;
                    sheet.Cell(row, 7).Value = item.Department;
                    sheet.Cell(row, 6).Value = item.Section;
                    sheet.Cell(row, 5).Value = item.Category;
                    sheet.Cell(row, 4).Value = item.ItemName;
                    sheet.Cell(row, 3).Value = item.EntitledQty;
                    sheet.Cell(row, 2).Value = item.IssuedQty;
                   


                    row++;
                }

                // ✅ AutoFit
                sheet.Columns().AdjustToContents();

                // ✅ Borders
                var tableRange = sheet.Range(1, 1, row - 1, 9);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // ✅ Freeze header row
                sheet.SheetView.FreezeRows(1);

                // ✅ Export bytes
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }




    }


    public class ExcelUploadResult
    {
        public int SuccessCount { get; set; }
        public List<string> Errors { get; set; }
    }

}

