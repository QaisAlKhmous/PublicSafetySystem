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
       static string GenerateEmployeeNumberGuid()
        {
            
            return "TEMP-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        }


        public static ExcelUploadResult AddEmployeesFromExcel(Stream fileStream)
        {
            var errors = new List<string>();
            var employees = new List<Employee>();
            var histories = new List<EmployeeJobTitleHistory>();

            // لمنع تكرار الرقم الوظيفي داخل نفس الملف
            var fileEmployeeNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var wb = new XLWorkbook(fileStream))
            {
                var ws = wb.Worksheet("EmployeesTemplate") ?? wb.Worksheet(1);

                // آخر صف فيه بيانات
                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                if (lastRow < 4)
                {
                    return new ExcelUploadResult
                    {
                        SuccessCount = 0,
                        Errors = new List<string> { "No employee data found in the file." }
                    };
                }

                for (int r = 4; r <= lastRow; r++)
                {
                    var row = ws.Row(r);

                   var employeeNumber = row.Cell(1).GetString().Trim();
                    //if (string.IsNullOrWhiteSpace(employeeNumber))
                    //    continue;

                    try
                    {
                        if (string.IsNullOrWhiteSpace(employeeNumber))
                            employeeNumber = GenerateEmployeeNumberGuid();

                        if (!fileEmployeeNumbers.Add(employeeNumber))
                        {
                            errors.Add($"Row {r}: الرقم الوظيفي ({employeeNumber}) مكرر داخل نفس الملف.");
                            continue;
                        }

                        if (EmployeeRepo.EmployeeNumberExists(employeeNumber))
                        {
                            errors.Add($"Row {r}: الرقم الوظيفي ({employeeNumber}) مستخدم مسبقاً.");
                            continue;
                        }


                        string firstName = row.Cell(2).GetString().Trim();
                        string secondName = row.Cell(3).GetString().Trim();
                        string lastName = row.Cell(4).GetString().Trim();

                        if (string.IsNullOrWhiteSpace(firstName) ||
                            string.IsNullOrWhiteSpace(lastName))
                        {
                            errors.Add($"Row {r}: الاسم الأول/الأب مطلوبين.");
                            continue;
                        }

                        string email = row.Cell(5).GetString().Trim();
                        string phone = row.Cell(6).GetString().Trim();

                        if (!TryReadDate(row.Cell(7), out DateTime employmentDate))
                        {
                            errors.Add($"Row {r}: تاريخ التعيين غير صحيح. استخدم yyyy-mm-dd أو تاريخ Excel.");
                            continue;
                        }

                        string workLocationStr = row.Cell(8).GetString().Trim();
                        if (!Enum.TryParse(workLocationStr, true, out enWorkLocation workLocation))
                        {
                            errors.Add($"Row {r}: موقع العمل غير صحيح ({workLocationStr}).");
                            continue;
                        }

                        string departmentName = row.Cell(9).GetString().Trim();
                        string sectionName = row.Cell(10).GetString().Trim();
                        string jobTitleName = row.Cell(11).GetString().Trim();

                        if (string.IsNullOrWhiteSpace(departmentName) ||
                            string.IsNullOrWhiteSpace(sectionName) ||
                            string.IsNullOrWhiteSpace(jobTitleName))
                        {
                            errors.Add($"Row {r}: الوحدة التنظيمية/القسم/المسمى الوظيفي مطلوبين.");
                            continue;
                        }

                        var dept = DepartmentService.GetDepartmentByName(departmentName);
                        if (dept == null)
                        {
                            errors.Add($"Row {r}: الوحدة التنظيمية غير صحيحة ({departmentName}).");
                            continue;
                        }

                        var section = SectionService.GetSectionByName(sectionName);
                        if (section == null)
                        {
                            errors.Add($"Row {r}: القسم غير صحيح ({sectionName}).");
                            continue;
                        }

                        // (اختياري) تأكد القسم تابع للوحدة إذا عندك DepartmentId في Section
                        // if (section.DepartmentId != dept.DepartmentId)
                        // {
                        //     errors.Add($"Row {r}: القسم ({sectionName}) لا يتبع للوحدة ({departmentName}).");
                        //     continue;
                        // }

                        var jobTitle = JobTitleService.GetJobTitleByName(jobTitleName);
                        if (jobTitle == null)
                        {
                            errors.Add($"Row {r}: المسمى الوظيفي غير صحيح ({jobTitleName}).");
                            continue;
                        }

                        var employeeId = Guid.NewGuid();

                        var employee = new Employee
                        {
                            EmployeeId = employeeId,
                            EmployeeNumber = string.IsNullOrWhiteSpace(employeeNumber)
                                     ? null
                                     : employeeNumber,

                            FirstName = firstName,
                            SecondName = secondName,
                            LastName = lastName,
                            FullName = $"{firstName} {secondName} {lastName}",

                            Email = email,
                            Phone = phone,

                            EmploymentDate = employmentDate,
                            WorkLocation = workLocation,

                            DepartmentId = dept.DepartmentId,
                            SectionId = section.SectionId,
                            JobTitleId = jobTitle.JobTitleId,

                            Active = true,
                            IsIntern = false,

                            CreationDate = DateTime.Now,
                            JobTitleUpdateDate = employmentDate
                        };

                        employees.Add(employee);

                        histories.Add(new EmployeeJobTitleHistory
                        {
                            EmployeeJobTitleHistoryId = Guid.NewGuid(),
                            EmployeeId = employeeId,
                            JobTitleId = jobTitle.JobTitleId,
                            StartDate = employmentDate,
                            EndDate = null
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {r}: {ex.Message}");
                    }
                }
            }

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

        private static bool TryReadDate(IXLCell cell, out DateTime date)
        {
            date = default;

            // إذا الخلية تاريخ Excel حقيقي
            if (cell.DataType == XLDataType.DateTime)
            {
                date = cell.GetDateTime();
                return true;
            }

            // إذا مكتوبة كنص
            var s = cell.GetString().Trim();
            if (string.IsNullOrWhiteSpace(s)) return false;

            // جرّب parsing مرن (yyyy-mm-dd وغيرها)
            return DateTime.TryParse(s, out date);
        }




        public static byte[] GenerateEmployeeTemplate()
        {
           var workLocations = new[] { "Amman", "Khaldieh" };

            
            var deptList = DepartmentService.GetAllDepartments().Select(d => d.Name).ToList();
            var secList = SectionService.GetAllSections().Select(s => s.Name).ToList();
            var jobList = JobTitleService.GetAllJobTitles().Select(j => j.Name).ToList();
            var locList = (workLocations ?? Enumerable.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();

             var wb = new XLWorkbook();

         
            var ws = wb.Worksheets.Add("EmployeesTemplate");

            // Instructions row
            ws.Cell("A1").Value = "التعليمات: الحقول المميزة بعلامة * إلزامية. يجب إدخال التاريخ بصيغة yyyy-mm-dd. يرجى استخدام القوائم المنسدلة عند توفرها.";

            ws.Range("A1:K1").Merge().Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Fill.SetBackgroundColor(XLColor.LightYellow);

            // Headers (Row 2)
            ws.Cell("A2").Value = "Employee Number (رقم وظيفي)*";
            ws.Cell("B2").Value = "First Name (الاسم الأول)*";
            ws.Cell("C2").Value = "Second Name (اسم الأب)*";
            ws.Cell("D2").Value = "Last Name (اسم العائلة)*";
            ws.Cell("E2").Value = "Email (البريد الإلكتروني)";
            ws.Cell("F2").Value = "Phone (رقم الهاتف)";
            ws.Cell("G2").Value = "Employment Date (yyyy-mm-dd)*";
            ws.Cell("H2").Value = "Work Location (Amman/Khaldieh)*";
            ws.Cell("I2").Value = "Department Name (الوحدة التنظيمية)*";
            ws.Cell("J2").Value = "Section Name (القسم)*";
            ws.Cell("K2").Value = "Job Title (المسمى الوظيفي)*";

            // Header style
            var headerRange = ws.Range("A2:K2");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Required columns highlight (optional: subtle)
            // A,B,C,D,G,H,I,J,K
            foreach (var col in new[] { 1, 2, 3, 4, 7, 8, 9, 10, 11 })
                ws.Cell(2, col).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241); // light blue-ish

            // Column widths
            ws.Column(1).Width = 22;
            ws.Column(2).Width = 18;
            ws.Column(3).Width = 18;
            ws.Column(4).Width = 18;
            ws.Column(5).Width = 28;
            ws.Column(6).Width = 18;
            ws.Column(7).Width = 22;
            ws.Column(8).Width = 26;
            ws.Column(9).Width = 26;
            ws.Column(10).Width = 26;
            ws.Column(11).Width = 26;

            // Freeze panes (keep header visible)
            ws.SheetView.FreezeRows(2);

            // Example row (Row 3)
            ws.Cell("A3").Value = "1023";
            ws.Cell("B3").Value = "أحمد";
            ws.Cell("C3").Value = "محمد";
            ws.Cell("D3").Value = "صالح";
            ws.Cell("E3").Value = "ahmad@example.com";
            ws.Cell("F3").Value = "0790000000";
            ws.Cell("G3").Value = "2026-01-15";
            ws.Cell("H3").Value = "Amman";
            ws.Cell("I3").Value = deptList.FirstOrDefault() ?? "IT Department";
            ws.Cell("J3").Value = secList.FirstOrDefault() ?? "Development Section";
            ws.Cell("K3").Value = jobList.FirstOrDefault() ?? "Software Engineer";
            ws.Range("A3:K3").Style.Font.FontColor = XLColor.DarkGray;

            // Make data entry area (Rows 3..5000 for example)
            int startRow = 4;
            int endRow = 5000;

            // =========================
            // Lists Sheet (Hidden)
            // =========================
            var listsWs = wb.Worksheets.Add("Lists");
            listsWs.Visibility = XLWorksheetVisibility.VeryHidden;

            // Put lists in columns
            WriteList(listsWs, "A1", "WorkLocations", locList);
            WriteList(listsWs, "B1", "Departments", deptList);
            WriteList(listsWs, "C1", "Sections", secList);
            WriteList(listsWs, "D1", "JobTitles", jobList);

            // Define named ranges (for data validation)
            DefineNamedRange(wb, listsWs, "WorkLocations", "A2", locList.Count);
            DefineNamedRange(wb, listsWs, "Departments", "B2", deptList.Count);
            DefineNamedRange(wb, listsWs, "Sections", "C2", secList.Count);
            DefineNamedRange(wb, listsWs, "JobTitles", "D2", jobList.Count);

            // =========================
            // Data Validation
            // =========================

            // Work location dropdown (H)
            AddDropdown(ws.Range(startRow, 8, endRow, 8), "WorkLocations");

            // Department dropdown (I)
            if (deptList.Count > 0)
                AddDropdown(ws.Range(startRow, 9, endRow, 9), "Departments");

            // Section dropdown (J)
            if (secList.Count > 0)
                AddDropdown(ws.Range(startRow, 10, endRow, 10), "Sections");

        
            if (jobList.Count > 0)
                AddDropdown(ws.Range(startRow, 11, endRow, 11), "JobTitles");

            var dateRange = ws.Range(startRow, 7, endRow, 7);

         
            dateRange.Style.DateFormat.Format = "yyyy-mm-dd";

           
            var dvDate = dateRange.CreateDataValidation();
            dvDate.AllowedValues = XLAllowedValues.Date;
            dvDate.Operator = XLOperator.Between;

      
            dvDate.MinValue = "DATE(1900,1,1)";
            dvDate.MaxValue = "DATE(2100,12,31)";


            dvDate.InputTitle = "Employment Date";
            dvDate.InputMessage = "Enter date in format yyyy-mm-dd";
            dvDate.ErrorTitle = "Invalid Date";
            dvDate.ErrorMessage = "Please enter a valid date between 1900-01-01 and 2100-12-31.";

            // Email basic hint (optional)
            var emailRange = ws.Range(startRow, 5, endRow, 5);
            var dvEmail = emailRange.CreateDataValidation();
            dvEmail.InputTitle = "Email";
            dvEmail.InputMessage = "Example: name@domain.com";




            // Unlock data entry area so they can type
            ws.Range(startRow, 1, endRow, 11).Style.Protection.Locked = false;



            // Lock Row 1 (Instructions)
            ws.Range("1:1").Style.Protection.Locked = true;

            // Lock Row 2 (Headers)
            ws.Range("2:2").Style.Protection.Locked = true;

            // Lock Row 3 (Headers)
            ws.Range("3:3").Style.Protection.Locked = true;

            ws.Protect("1234");


            // Borders for the entry area (optional)
            var tableRange = ws.Range(2, 1, endRow, 11);
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

             var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private static void WriteList(IXLWorksheet ws, string headerCell, string header, IList<string> values)
        {
            ws.Cell(headerCell).Value = header;
            ws.Cell(headerCell).Style.Font.Bold = true;

            int col = ws.Cell(headerCell).Address.ColumnNumber;
            int row = ws.Cell(headerCell).Address.RowNumber + 1;

            for (int i = 0; i < values.Count; i++)
                ws.Cell(row + i, col).Value = values[i];
        }

        private static void DefineNamedRange(XLWorkbook wb, IXLWorksheet ws, string rangeName, string startCell, int count)
        {
           
            if (count <= 0) return;

            var start = ws.Cell(startCell).Address;
            var end = ws.Cell(start.RowNumber + count - 1, start.ColumnNumber).Address;
            var rng = ws.Range(start, end);

           
            var existing = wb.DefinedNames.FirstOrDefault(n => n.Name.Equals(rangeName, StringComparison.OrdinalIgnoreCase));
            existing?.Delete();

            wb.DefinedNames.Add(rangeName, rng);
        }

        private static void AddDropdown(IXLRange range, string namedRange)
        {
            var dv = range.CreateDataValidation();
            dv.AllowedValues = XLAllowedValues.List;
            dv.InCellDropdown = true;
            dv.List(namedRange, true);
            dv.ErrorTitle = "Invalid Value";
            dv.ErrorMessage = "Please select a value from the dropdown list.";
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

