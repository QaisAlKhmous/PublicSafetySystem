using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class ChangeRequestController : Controller
    {
        [HttpPost]
        public JsonResult AddNewChangeRequest(ChangeRequestDTO ChangeRequest)
        {
            ChangeRequestService.AddNewChangeRequest(ChangeRequest);

            return Json(new { success = true });
        }
        [HttpGet]
        public JsonResult GetAllChangeRequests()
        {
            return Json(ChangeRequestService.GetAllChangeRequests(),JsonRequestBehavior.AllowGet);
        }
    }
}
