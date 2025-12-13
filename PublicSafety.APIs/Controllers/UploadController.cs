using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class UploadController: Controller
    {
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return Json(new { success = false, message = "No file uploaded" });

            string uploadsFolder = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string extension = Path.GetExtension(file.FileName);
            string fileName = $"file_{Guid.NewGuid():N}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            file.SaveAs(filePath);

            return Json(new
            {
                success = true,
                fileName = fileName
            });
        }


        [HttpGet]
        public ActionResult Download(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return new HttpStatusCodeResult(400);

            fileName = Path.GetFileName(fileName); // security

            string fullPath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);

            if (!System.IO.File.Exists(fullPath))
                return HttpNotFound("File not found: " + fileName);

            // FORCE download (no browser preview)
            return File(
                fullPath,
                "application/octet-stream",
                fileName
            );
        }


        [HttpPost]
        public JsonResult UploadEmployeesExcel(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return Json(new { SuccessCount = 0, Errors = new List<string> { "No file uploaded" } });

            var result = ExcelService.AddEmployeesFromExcel(file.InputStream);

            return Json(result);
        }
        public FileResult DownloadEmployeeTemplate()
        {
            var fileBytes = ExcelService.GenerateEmployeeTemplate();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmployeeUploadTemplate.xlsx");
        }


        [HttpGet]
        public ActionResult DownloadAllEmployees()
        {
            var fileContents = ExcelService.ExportAllEmployeesToExcel();
            var fileName = $"Employees_{DateTime.Now:yyyyMMddHHmm}.xlsx";

            return File(fileContents,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }

    }
}
