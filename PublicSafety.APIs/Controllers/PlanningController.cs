using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class PlanningController : Controller
    {
        [HttpGet]
        public JsonResult Overview(int fromYear, int toYear)
        {
            var data = PlanningService.GetOverview(fromYear, toYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PlannedItemsByYear(int year)
        {
            var data = PlanningService.GetPlannedItemDetails(year, year);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
