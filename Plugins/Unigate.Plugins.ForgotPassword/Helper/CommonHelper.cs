using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using MCMS.Common.Extension;
using Mcms.UI.Framework.PluginArea;

namespace Ayyapim.Plugins.ForgotPassword.Helper
{
    public class CommonHelper
    {
        public static bool IsMailAddress(string emailaddress)
        {
            if (emailaddress.IsNull())
                return false;
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static void SendMail(string title, string body, string mailAddress)
        {
            //try
            //{
            WebMail.SmtpServer = PluginContext.Current.GetProperty("SmtpServer");//  "smtp.gmail.com";
            WebMail.EnableSsl = PluginContext.Current.GetProperty("SecureConnection").ToModel<bool>();

            WebMail.UserName = PluginContext.Current.GetProperty("MailUserName"); //"ayyapimbilgi@gmail.com";
            WebMail.Password = PluginContext.Current.GetProperty("MailPassword"); //"123ayyapim";
            WebMail.SmtpPort = PluginContext.Current.GetProperty("SmtpPort").ToInt(); //587;
            WebMail.SmtpUseDefaultCredentials = false;
            WebMail.Send(mailAddress, title, body);
            //}
            //catch (Exception ex)
            //{
            //    return true;
            //}
        }

    }


}