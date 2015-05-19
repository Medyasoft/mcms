using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Mcms.Plugins.AdminSiteRedirect.Models;
using Mcms.Sdk;
using Mcms.UI.Framework.Utilities;
using Mcms.UI.Framework.Web.Mvc.Routing;
using MCMS.Common.Models.ViewModels;
using MCMS.Common.Runtime;
using MCMS.Common.Runtime.Dependency;
using MCMS.Common.Runtime.Mvc;



namespace Mcms.Plugins.AdminSiteRedirect.Module
{
    [Dependency(typeof(IHttpApplicationEvents), Key = "SiteRedirectModule", Order = 1)]
    public class SiteRedirectModule : HttpApplicationEvents
    {
        private const string AppRedirectName = "REDIRECTS";
        static readonly object ObjectLockSiteRedirect = new object();

        public override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            var context = HttpContext.Current;
            var application = context.Application;

            FillRedirects();
            //var siteRedirects = UnigateObject.Query("SiteRedirect").WhereLike("LocalAddress", "*").ToList<SiteRedirectModel>();
            //application["SiteRedirectsWithAsterisk"] = siteRedirects;
        }

        void FillRedirects()
        {
            var siteRedirects = UnigateObject.Query("SiteRedirect").ToList<SiteRedirectModel>();
            HttpContext.Current.Application[AppRedirectName] = siteRedirects;
        }

        public override void Application_BeginRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var request = context.Request;
            var response = context.Response;
            var application = context.Application;
           
            string domainName = RouteUtility.DomainName(context);
            var allPages = PageUtility.GetAllPages();
            var currentPage = allPages.FirstOrDefault(x => x.RouteValue == domainName);

            if (currentPage == null) return;

            if (request.Url.ToString().Contains(".ashx") || request.Url.ToString().Contains(".axd") || request.Url.ToString().Contains(".asmx") || request.Url.ToString().Contains("DataSource")) return;

            base.Application_BeginRequest(sender, e);

            #region Check RouteMap

            var siteMapLastUpdate = (SiteMapLastUpdateViewModel)application["SiteMapLastUpdate"];

            var siteMapLastUpdateFromService = UnigateObject.Query("SiteMapLastUpdate").FirstOrDefault<SiteMapLastUpdateViewModel>();

            if (siteMapLastUpdateFromService.SiteRedirectLastUpdateDate > siteMapLastUpdate.SiteRedirectLastUpdateDate)
            {
                if (Monitor.TryEnter(ObjectLockSiteRedirect))
                {
                    if (siteMapLastUpdateFromService.SiteRedirectLastUpdateDate > siteMapLastUpdate.SiteRedirectLastUpdateDate)
                    {
                        FillRedirects();

                        siteMapLastUpdate.SiteLanguageLastUpdateDate = siteMapLastUpdateFromService.SiteRedirectLastUpdateDate;
                        application["SiteMapLastUpdate"] = siteMapLastUpdate;
                    }
                    Monitor.Exit(ObjectLockSiteRedirect);
                }
            }
            #endregion

            #region Site Redirect Check
            var redirects = application[AppRedirectName] as List<SiteRedirectModel>;

            if (redirects == null)
            {
                redirects = new List<SiteRedirectModel>();
            }

            var redirectUrl = redirects.FirstOrDefault(i => i.LocalAddress == request.Path.ToLowerInvariant());

            if (redirectUrl != null && !string.IsNullOrEmpty(redirectUrl.LocalAddress))
            {
                //İlgili domain için yönlendirme yapılmalı
                if (redirectUrl.PageId != currentPage.Id) return;

                var siteRedirect = redirectUrl;
                if (siteRedirect.TransferQuerystring)
                {
                    if (!siteRedirect.IsTemporary)
                        response.RedirectPermanent(siteRedirect.RouteAddress + request.Url.Query);
                    else
                        response.Redirect(siteRedirect.RouteAddress + request.Url.Query);
                }
                else
                {
                    if (siteRedirect.IsTemporary == true)
                        response.RedirectPermanent(siteRedirect.RouteAddress);
                    else
                        response.Redirect(siteRedirect.RouteAddress);
                }
            }
            else
            {
                var siteRedirectsWithAsterisk = (application[AppRedirectName] as List<SiteRedirectModel>).Where(x => x.LocalAddress.Contains("*"));
                var siteRedirect = siteRedirectsWithAsterisk.FirstOrDefault(delegate(SiteRedirectModel s)
                {
                    var regexString = String.Format(@"^{0}$", s.LocalAddress.Trim().Replace("*", "(.*)"));

                    var isMached = Regex.IsMatch(request.Path.ToLowerInvariant(), regexString);
                    return isMached;

                });

                if (siteRedirect != null)
                {
                    //İlgili domain için yönlendirme yapılmalı
                    if (siteRedirect.PageId != currentPage.Id) return;

                    var is301 = !siteRedirect.IsTemporary;
                    string responseStatus = string.Empty;
                    if (is301)
                        responseStatus = "301 Moved Permanently";
                    else
                        responseStatus = "302 Moved Temprorarily";

                    response.Status = responseStatus;
                    if (siteRedirect.TransferQuerystring)
                    {
                        if (is301)
                            response.RedirectPermanent(siteRedirect.RouteAddress + request.Url.Query);
                        else
                            response.Redirect(siteRedirect.RouteAddress + request.Url.Query);
                    }
                    else
                    {
                        if (is301)
                            response.RedirectPermanent(siteRedirect.RouteAddress);
                        else
                            response.Redirect(siteRedirect.RouteAddress);
                    }
                }
                else
                {
                    application["Redirects"] = redirects;
                }
            }

            #endregion
        }
    }
}

