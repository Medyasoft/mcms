using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Mcms.Plugins.ForgotPassword.Models;

namespace Ayyapim.Plugins.ForgotPassword.Helper
{
    public class MailService
    {
        /// <summary>
        ///     SendMail
        ///     Birden fazla mail adresine gönderimi için mail address param : mail1@mail.com,mail2@mail.com,mail3@mail.com,...
        /// </summary>
        /// <param name="fromMailAdress"></param>
        /// <param name="fromDisplayName"></param>
        /// <param name="toAddress"></param>
        /// <param name="ccAddress"></param>
        /// <param name="bccAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string SendMail(string fromMailAdress, string fromDisplayName, string toAddress,
            List<string> ccAddress, string bccAddress, string subject, string body)
        {
            if (string.IsNullOrEmpty(body))
                return string.Empty;

            string result = "";
            string smtpServer = "";
            int smtpPort = 25;
            bool enableSsl = false;
            string username = "";
            string password = "";


            smtpServer = MailSetting.Current.SmtpServer;
            smtpPort = MailSetting.Current.SmtpPort != 0 ? MailSetting.Current.SmtpPort : 25;
            enableSsl = MailSetting.Current.EnableSsl ? true : false;
            username = MailSetting.Current.Username;
            password = MailSetting.Current.Password;

            var mail = new MailMessage();
            mail.From = String.IsNullOrEmpty(MailSetting.Current.Username)
                ? new MailAddress(MailSetting.Current.Username)
                : new MailAddress(MailSetting.Current.Username, MailSetting.Current.Username);
            mail.To.Add(toAddress);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            if (ccAddress.Count > 0)
            {
                foreach (string str in ccAddress)
                {
                    mail.CC.Add(str);
                }
            }

            if (!string.IsNullOrEmpty(bccAddress))
                mail.Bcc.Add(bccAddress);

            var smtp = new SmtpClient();
            smtp.Host = smtpServer;
            smtp.Port = smtpPort;
            smtp.EnableSsl = enableSsl;

            if (String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
                smtp.UseDefaultCredentials = true;
            else
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(username, password);
            }

            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public static string SendMail(string fromMailAdress, string toAddress, string subject, string body)
        {
            return SendMail(fromMailAdress, "", toAddress, new List<string>(), "", subject, body);
        }


        public static string SendMail(string fromMailAdress, string fromDisplayName, string toAddress, string cc,
            string subject, string body)
        {
            return SendMail(fromMailAdress, fromDisplayName, toAddress, new List<string>(), "", subject, body);
        }
    }
}