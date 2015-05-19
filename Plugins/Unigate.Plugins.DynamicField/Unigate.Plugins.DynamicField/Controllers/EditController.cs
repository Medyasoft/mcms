using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mcms.Sdk;
using Mcms.UI.Framework;
using Mcms.UI.Framework.Authorization;
using Mcms.UI.Framework.PluginArea;
using Mcms.UI.Framework.PluginAttribute;
using Unigate.Plugins.DynamicField.Entities;

namespace Unigate.Plugins.DynamicField.Controllers
{
    [AdminController(Name = "Dynamic Field")]
    //[Authorization(Role = "Admin")]
    public class EditController : Controller
    {
        // GET: Home
        [UIAction(Name = "Dynamic Field")]
        public ActionResult Index(string key)
        {
            Field field = UnigateObject.Query("DynamicField")
                .WhereEqualTo("FieldKey", key)
                .ToList<Field>()
                .FirstOrDefault();

            return View(field ?? new Field() { FieldKey = key });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(Field field, FormCollection formCollection)
        {
            var fieldValue = formCollection["value"];
            field.FieldValue = fieldValue;

            var _field = UnigateObject.Query("DynamicField")
                .WhereEqualTo("FieldKey", field.FieldKey)
                .WhereEqualTo("SiteLanguageId", field.SiteLanguageId)
                .WhereEqualTo("PageId", field.PageId)
                .ToList<Field>()
                .FirstOrDefault();

            if (_field == null)
            {
                UnigateObject.Insert<Field>("DynamicField", field).Execute();
            }
            else
            {
                UnigateObject.Update("DynamicField")
                    .Column("FieldValue", field.FieldValue)
                    .WhereEqualTo("ContentId", _field.ContentId)
                    .Execute();
            }

            return new JsonResult
            {
                Data = field,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}