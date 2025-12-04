using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class JobTitleController : Controller
    {
        [HttpGet]
        public JsonResult GetAllJobTitles()
        {
            var JobTitles = JobTitleService.GetAllJobTitles();

            return Json(JobTitles, JsonRequestBehavior.AllowGet);

        }

    }
}
