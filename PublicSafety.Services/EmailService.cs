using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdminDashboard.Services
{
    public class EmailService
    {

        private static string GetEntityTypeArabic(enEntityType type)
        {
            switch (type)
            {
                case enEntityType.Employee:
                    return "الموظف";

                case enEntityType.Item:
                    return "الصنف";

                case enEntityType.Matrix:
                    return "المصفوفة";

                case enEntityType.Issuance:
                    return "الصرف";

                default:
                    return "غير معروف";
            }
        }
        public static void NotifyAdminsForApproval(ChangeRequest request)
        {
            string systemUrl =
    ConfigurationManager.AppSettings["SystemBaseUrl"];

            string returnUrl = "/Home/Index#!/requests";

            string link =
                $"{systemUrl}/Account/Login?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}";

            var subject = "طلب تعديل جديد بانتظار الموافقة";

            var body = $@"
تم تقديم طلب تعديل جديد في النظام ويحتاج إلى موافقتكم.

بيانات الطلب:
------------------------
المستخدم: {UserService.GetUserByUserId(request.ChangedById).Username}
نوع الطلب: {GetEntityTypeArabic(request.EntityType)}
رقم الطلب: {request.RequestId}
تاريخ الطلب: {request.RequestDate:yyyy-MM-dd HH:mm}

يرجى الدخول إلى النظام عبر الرابط التالي لمراجعة الطلب:
{link}


";

 


            //foreach (var adminEmail in admins)
            {
                SendEmail("qmohammad.kh@gmail.com", subject, body);
            }
        }

        private static void SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.From = new MailAddress("noreply@yourdomain.com", "مستودع السلامة العامة");
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("qmohammad.kh@gmail.com", "wmjx tjqo zvpb ornz"),
                EnableSsl = true
            };

            smtp.Send(message);
        }
    }
}
