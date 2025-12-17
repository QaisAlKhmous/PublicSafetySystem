using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class DepartmentController : Controller
    {

        [HttpGet]
        public JsonResult GetAllDepartments()
        {
            try
            {
                var departments = DepartmentService.GetAllDepartments();

                return Json(new ApiResponse<IEnumerable<DepartmentDTO>>
                {
                    Success = true,
                    Message = null,
                    Data = departments ?? new List<DepartmentDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<IEnumerable<DepartmentDTO>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<DepartmentDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
