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
    public class IssuanceController : Controller
    {
        public JsonResult AddNewIssuance(AddIssuanceDTO issaunce)
        {
            IssuanceService.AddNewIssuance(issaunce);

            return Json("Issued successfuly!");
        }
    }
}
