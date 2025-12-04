using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class MatrixController : Controller
    {
        [HttpGet]
        public JsonResult GetAllMatrices()
        {
            var items = MatrixService.GetAllMatrices();

            return Json(items, JsonRequestBehavior.AllowGet);


        }
    }
}
