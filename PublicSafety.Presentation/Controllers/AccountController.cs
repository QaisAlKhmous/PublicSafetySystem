using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PublicSafety.Presentation.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            var LogedInUser = UserService.Login(Username, Password);

            if (LogedInUser == null)
            {

                ViewBag.LoginError = "اسم المستخدم او كلمة لمرور خاطئة";

                return View();
            }


            if (!LogedInUser.IsPassword)
            {
                ViewBag.LoginError = "اسم المستخدم او كلمة لمرور خاطئة";


                return View();
            }


           

           

            HttpCookie userCookie = new HttpCookie("UserInfo");
            userCookie.HttpOnly = false;
            userCookie["Username"] = Username;
            userCookie["UserId"] = LogedInUser.UserId.ToString();
            userCookie["Type"] = LogedInUser.Type.ToString();
            userCookie.Expires = DateTime.Now.AddHours(1);
            Response.Cookies.Add(userCookie);


            return RedirectToAction("Index", "Home");




        }


        public ActionResult Logout()
        {
            if (Request.Cookies["UserInfo"] != null)
            {
                Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

            return RedirectToAction("Login", "Account");
        }
    }
}