using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class SectionController : Controller
    {

        [HttpGet]
        public JsonResult GetAllSections()
        {
            var Sections = SectionService.GetAllSections();

            return Json(Sections, JsonRequestBehavior.AllowGet);

        }


    }
}
