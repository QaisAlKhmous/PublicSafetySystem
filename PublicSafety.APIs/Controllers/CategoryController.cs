using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        public JsonResult GetAllCategories()
        {
            var Categories = CategoryService.GetAllCategories();

            return Json(Categories, JsonRequestBehavior.AllowGet);

        }
    }
}
