using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class UserController : Controller
    {

        [HttpPost]
        public ActionResult Login(string Username,string Password)
        {
            LoginResultDTO loginResult = UserService.Login(Username, Password);
            if (loginResult == null || !loginResult.IsPassword)
            {
                return Json(new { success = false, message = "اسم المستخدم او كلمة المرور خاطئة" });
            }
           
                return Json(new
                {
                    success = true,
                    userId = loginResult.UserId,
                    username = loginResult.Username,
                    role = loginResult.Type
                });
           
        }
    }
}
