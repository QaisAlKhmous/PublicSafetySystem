using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class ItemLogController : Controller
    {
        [HttpGet]
        public JsonResult ByItem(Guid itemId)
        {
            var logs = ItemLogService.GetItemLogsByItem(itemId);
            return Json(logs,JsonRequestBehavior.AllowGet);
        }
    }
}
