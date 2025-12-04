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
    public class DisposalController : Controller
    {

        [HttpPost]
        public JsonResult AddDisposal(DisposalDTO disposal)
        {

            
            DisposalService.AddNewDisposal(disposal);

            return Json("Added Successfully!");


        }

    }
}
