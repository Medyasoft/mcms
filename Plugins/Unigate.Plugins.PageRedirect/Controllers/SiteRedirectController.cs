using System.Web.Mvc;
using Mcms.Plugins.AdminSiteRedirect.Models;
using Mcms.Sdk;
using Mcms.UI.Framework.Authorization;
using Mcms.UI.Framework.PluginAttribute;
using MCMS.Common.Models.ViewModels;
using System.Linq;
using Kendo.Mvc.UI;
using System;

namespace Mcms.Plugins.AdminSiteRedirect.Controllers
{
    [AdminController(Name = "SiteRedirects Controller")]
    [Authorization(Role = "Admin")]
    public class SiteRedirectController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SiteRedirects_Read([DataSourceRequest] DataSourceRequest request)
        {
            var model = UnigateObject.Query("SiteRedirect").Join("Page", "Page.Id=SiteRedirect.PageId").ToList<SiteRedirectModel>();

            var result = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SiteRedirect_Delete(SiteRedirectModel siteRedirectModel)
        {
            var result = UnigateObject.Delete("SiteRedirect")
                .WhereEqualTo("ContentId", siteRedirectModel.ContentId)
                .Execute();

            return View();
        }

        public ActionResult New()
        {
            var sites = UnigateObject.Query("Page")
                .WhereEqualTo("Page.ParentPageId", null)
                .ToList<PageViewModel>();

            ViewData["Sites"] = sites.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString()
            });

            return View(new SiteRedirectModel());
        }

        public ActionResult Edit(Guid id)
        {
            var sites = UnigateObject.Query("Page")
                .WhereEqualTo("ParentPageId", null)
                .ToList<PageViewModel>();

            var redirect = UnigateObject.Query("SiteRedirect")
                .WhereEqualTo("ContentId", id)
                .FirstOrDefault<SiteRedirectModel>();

            ViewData["Sites"] = sites.Select(x => new SelectListItem
            {
                Selected = redirect.PageId == x.Id,
                Text = x.Title,
                Value = x.Id.ToString()
            });

            return View(redirect);
        }

        [HttpPost]
        public ActionResult Save(SiteRedirectModel model)
        {
            model.Title = model.LocalAddress;

            if (model.ContentId == Guid.Empty)
            {
                Insert insertRedirect = UnigateObject.Insert("SiteRedirect", model);
                var result = insertRedirect.Execute();
            }
            else
            {
                var result = UnigateObject.Update("SiteRedirect")
                    .Column("PageId", model.PageId)
                    .Column("Title", model.Title)
                    .Column("RouteAddress", model.RouteAddress)
                    .Column("LocalAddress", model.LocalAddress)
                    .Column("IsTemporary", model.IsTemporary)
                    .Column("TransferQuerystring", model.TransferQuerystring)
                    .WhereEqualTo("ContentId", model.ContentId).Execute();

                ViewBag.Result = result.ResultMessage;
            }

            //SiteMapLastUpdate tablosu güncelleniyor.
            UnigateObject.Update("SiteMapLastUpdate")
                .Column("SiteRedirectLastUpdateDate", DateTime.Now).Execute();

            return RedirectToAction("Index");
        }
    }
}