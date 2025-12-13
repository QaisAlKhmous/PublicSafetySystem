using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicSafety.Presentation.Controllers
{
    public class HomeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
            response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

            // simple auth check
            if (Request.Cookies["UserInfo"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
        public ActionResult Index()
        {
          
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }



        public ActionResult Items()
        {
            return View();
        }

        public ActionResult Matrices()
        {
            return View();
        }

        public ActionResult Employees()
        {
            return View();
        }

        public ActionResult AddEmployee()
        {
            return View();
        }
        public ActionResult Issuances()
        {
            return View();
        }
        public ActionResult Entitlements()
        {
            return View();
        }
        public ActionResult ItemLogs()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}