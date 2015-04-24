using System;
using System.Web;
using Ayyapim.Plugins.ForgotPassword.DataServices;
using MCMS.Common.Runtime;
using MCMS.Common.Runtime.Dependency;

namespace Mcms.Plugins.ForgotPassword.HttpApplicationService
{
    [Dependency(typeof(IHttpApplicationEvents), ComponentLifeStyle.Singleton, Key = "ForgotPassword.IOC", Order = -1)]
    public class IocApplicationEvents : HttpApplicationEvents
    {
        public override void Application_Start(object sender, EventArgs e)
        {
            EngineContext.Current.ContainerManager.AddComponent<IGeneralSerivce, GeneralService>();
        }

        //public override void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    var url = HttpContext.Current.Request.Url.AbsoluteUri.Split(new[] { '?' })[0];
        //    if (url.StartsWith("http://pw.wahooy.com"))
        //    {
        //        if (!url.StartsWith("http://pw.wahooy.com/plugins/ResetPassword"))
        //        {
        //            HttpContext.Current.Response.End();
        //        }
        //    }
        //}
    }
}