using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard.Services
{
    public class EmailService
    {
        public static void SendResetPasswordEmail(string to, string subject, string name, string password)
        {
            var client = new SmtpClient();
            client.EnableSsl = true;

            var mail = new MailMessage();
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;



            string htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Segoe UI, Tahoma, Geneva, Verdana, sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' border='0'>
    <tr>
        <td align='center' style='padding:50px 0;'>
            <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff; padding:30px; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
                <tr>
                    <td>
                        <h1 style='color:#4CAF50; margin:0 0 20px 0;'>Hello {name},</h1>
                        <p style='font-size:16px; line-height:1.5; color:#333333; margin:0 0 20px 0;'>Your password has been successfully reset. You can now log in with your new password.</p>
                        <p style='font-size:16px; line-height:1.5; color:#333333; margin:0 0 20px 0;'>Your new password is <span style='color:#4CAF50;'>{password}</span>.</p>
                        <a href='https://localhost:44312/#!/' style='display:inline-block; padding:10px 20px; background-color:#4CAF50; color:#ffffff; text-decoration:none; border-radius:5px;'>Login Now</a>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

</body>
</html>
";
            mail.Body = htmlBody;

            client.Send(mail);
        }





        public static void SendEmail(List<string> toEmails, string subject, string body)
        {
            var client = new SmtpClient();
            client.EnableSsl = true;

            var mail = new MailMessage();
            foreach (var Email in toEmails)
            {
                mail.To.Add(Email);
            }

            mail.Subject = subject;
            mail.IsBodyHtml = true;



            string htmlBody = body;
            mail.Body = htmlBody;

            client.Send(mail);
        }

        public static void SendRegApprovedEmail(string to, string subject, string name)
        {
            var client = new SmtpClient();
            client.EnableSsl = true;

            var mail = new MailMessage();
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;



            string htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Segoe UI, Tahoma, Geneva, Verdana, sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' border='0'>
    <tr>
        <td align='center' style='padding:50px 0;'>
            <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff; padding:30px; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
                <tr>
                    <td>
                        <h1 style='color:#4CAF50; margin:0 0 20px 0;'>Hello {name},</h1>
                        <p style='font-size:16px; line-height:1.5; color:#333333; margin:0 0 20px 0;'>We are glad to tell you that your registeration request has beed <span style='color:#4CAF50;'>approved</span>. You can now log in.</p>
                       
                        <a href='https://localhost:44312/#!/' style='display:inline-block; padding:10px 20px; background-color:#4CAF50; color:#ffffff; text-decoration:none; border-radius:5px;'>Login Now</a>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

</body>
</html>
";
            mail.Body = htmlBody;

            client.Send(mail);
        }

        public static void SendRegRejectedEmail(string to, string subject, string name)
        {
            var client = new SmtpClient();
            client.EnableSsl = true;

            var mail = new MailMessage();
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;



            string htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Segoe UI, Tahoma, Geneva, Verdana, sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' border='0'>
    <tr>
        <td align='center' style='padding:50px 0;'>
            <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff; padding:30px; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
                <tr>
                    <td>
                        <h1 style='color:#4CAF50; margin:0 0 20px 0;'>Hello {name},</h1>
                        <p style='font-size:16px; line-height:1.5; color:#333333; margin:0 0 20px 0;'>We are sorry to tell you that your registeration request has beed <span style='color:#4CAF50;'>rejected</span>.</p>
                        <p style='font-size:16px; line-height:1.5; color:#333333; margin:0 0 20px 0;'>Contact admin if you have any concerns</p>
   
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

</body>
</html>
";
            mail.Body = htmlBody;

            client.Send(mail);
        }

        public static void SendUnlockedEmail(string to, string subject, string name)
        {
            var client = new SmtpClient();
            client.EnableSsl = true;

            var mail = new MailMessage();
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;



            string htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Segoe UI, Tahoma, Geneva, Verdana, sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' border='0'>
    <tr>
        <td align='center' style='padding:50px 0;'>
            <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff; padding:30px; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
                <tr>
                    <td>
                        <h1 style='color:#4CAF50; margin:0 0 20px 0;'>Hello {name},</h1>
                        <p style='font-size:16px; line-height:1.5; color:#333333; margin:0 0 20px 0;'>Your account is <span style='color:#4CAF50;'>unlocked</span>. You can now log in.</p>
                       
                        <a href='https://localhost:44312/#!/' style='display:inline-block; padding:10px 20px; background-color:#4CAF50; color:#ffffff; text-decoration:none; border-radius:5px;'>Login Now</a>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

</body>
</html>
";
            mail.Body = htmlBody;

            client.Send(mail);
        }
    }
}
