using PublicSafety.Services;
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
            var Departments = DepartmentService.GetAllDepartments();

            return Json(Departments, JsonRequestBehavior.AllowGet);

        }


    }
}
