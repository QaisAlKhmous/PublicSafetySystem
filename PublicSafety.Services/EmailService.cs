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

            var admins = UserService.GetAdmins();

            foreach (var item in admins)
            {
                SendEmail(item.Email, subject, body);
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


        public static void SendIssueNotificationToManager(
     Guid userId)
        {
          

        
            var user = UserService.GetUserByUserId(userId);

            string issuedBy = user != null
                ? user.Username
                : "مستخدم غير معروف";

            string subject = "إشعار بعملية صرف مستحقات";

            string body =
                "السيد/ة المدير المحترم،\n\n" +
                "نود إعلامكم بأنه تم تنفيذ عملية صرف مستحقات للموظفين عبر نظام مستودع السلامة العامة.\n\n" +
                $"تمت العملية بواسطة المستخدم: {issuedBy}\n" +
                $"تاريخ العملية: {DateTime.Now:yyyy-MM-dd HH:mm}\n\n" +
                "يرجى العلم بأن هذا البريد للإشعار فقط ولا يتطلب أي إجراء.\n\n" +
                "مع الاحترام،\n" +
                "نظام مستودع السلامة العامة";

            var admins = UserService.GetAdmins();

            foreach (var item in admins)
            {
                SendEmail(item.Email, subject, body);
            }
            
        }



        public static void SendExceptionNotificationToManager(
    Guid userId,
    string employeeName,
    string employeeNumber,
    string itemName,
    int quantity,
    string reason,
    string exceptionFormPath)
        {
            
            var user = UserService.GetUserByUserId(userId);

            string issuedBy = user != null ? user.Username : "مستخدم غير معروف";

            string subject = "إشعار بصرف بدل استثناء لموظف";

            string body =
                "السيد/ة المدير المحترم،\n\n" +
                "نود إعلامكم بأنه تم تنفيذ عملية صرف بدل استثناء عبر نظام مستودع السلامة العامة.\n\n" +

                "تفاصيل الموظف:\n" +
                "--------------------------------------\n" +
                $"اسم الموظف: {employeeName}\n" +
                $"الرقم الوظيفي: {employeeNumber}\n\n" +

                "تفاصيل الاستثناء:\n" +
                $"المادة: {itemName}\n" +
                $"الكمية: {quantity}\n" +
                $"السبب: {reason}\n\n" +

                "معلومات العملية:\n" +
                $"تمت العملية بواسطة: {issuedBy}\n" +
                $"تاريخ العملية: {DateTime.Now:yyyy-MM-dd HH:mm}\n" +
                "--------------------------------------\n\n" +

                "مرفق نموذج الاستثناء.\n\n" +
                "هذا البريد للإشعار فقط.\n\n" +
                "مع الاحترام،\n" +
                "نظام مستودع السلامة العامة";

            var admins = UserService.GetAdmins();

            foreach (var item in admins)
            {
                SendEmail(item.Email, subject, body);
            }
        }

    }
}
