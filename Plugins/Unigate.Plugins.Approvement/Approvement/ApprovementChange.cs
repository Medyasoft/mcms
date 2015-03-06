using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;
using MCMS.Common.Result;
using MCMS.Common.Runtime.Dependency;
using MCMS.Common.Runtime.EventChange;

namespace Unigate.Plugins.Approvement.Approvement
{
    [Dependency(typeof(IDataChange), Key = "ApprovementChange", Order = 0)]
    public class ApprovementChange : DataChange
    {
        public override ApproveAfterResult AfterApprove(string listName, NameValueCollection formData, Guid insertedContentId, Guid siteLanguageId, DataState state)
        {
            var result = new ApproveAfterResult(ResultType.Successfull);
            var isSended = SendMail("hayrullahguven@gmail.com");
            Alert alert;
            if (isSended)
                alert = new Alert("Mesajınız iletildi", NotificationType.Information);
            else
                alert = new Alert("Mesajınız iletilemedi", NotificationType.Error);

            result.Alerts.Add(alert);

            return result;
        }

        public static bool SendMail(string emil)
        {
            var msg = new System.Net.Mail.MailMessage();
            SmtpClient sc = new SmtpClient();
            sc.Port = 465;
            sc.Host = "smtp.gmail.com";
            sc.EnableSsl = true;

            try
            {
                sc.Credentials = new System.Net.NetworkCredential("testmedyasoft@gmail.com", "qwer1234.");
                msg.To.Add(emil);
                msg.From = new System.Net.Mail.MailAddress("testmedyasoft@gmail.com", "Onaylama Mesajı", Encoding.UTF8);
                msg.Subject = "Mail Gönderme Testi";
                msg.SubjectEncoding = Encoding.UTF8;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Body = "Unigate Onaylama Mekanizması";
                sc.Send(msg);
                msg.Dispose();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
            finally
            {
                msg.Dispose();
            }
        }
    }
}