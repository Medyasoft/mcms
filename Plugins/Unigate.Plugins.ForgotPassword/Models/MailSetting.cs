using MCMS.Common.Extension;
using Mcms.UI.Framework.PluginArea;

namespace Mcms.Plugins.ForgotPassword.Models
{
    public class MailSetting
    {
        private static MailSetting _setting;
        private static MailSetting Instance()
        {
            var setting1 = new MailSetting
               {
                   SmtpServer = PluginContext.Current.GetProperty("SmtpServer"),
                   SmtpPort = PluginContext.Current.GetProperty("SmtpPort").ToModel<int>(),
                   EnableSsl = PluginContext.Current.GetProperty("SecureConnection").ToModel<bool>(),
                   Username = PluginContext.Current.GetProperty("MailUserName"),
                   Password = PluginContext.Current.GetProperty("MailPassword")
               };
            return setting1;
        }

        public static MailSetting Current
        {
            get
            {
                if (_setting == null)
                {
                    _setting = Instance();
                }
                return _setting;
            }
        }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}