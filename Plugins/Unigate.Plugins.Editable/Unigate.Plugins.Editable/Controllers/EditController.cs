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
using Unigate.Plugins.Editable;
using Unigate.Plugins.Editable.Dal;

namespace Unigate.Plugins.Editable.Controllers
{
    [AdminController(Name = "Düzenlenebilir Etiket")]
    //[Authorization(Role = "Admin")]
    public class EditController : Controller
    {
        //// GET: Home
        //[UIAction(Name = "Düzenlenbilir Etiket")]
        //public ActionResult Index(string key)
        //{
        //    Field field = Data.GetFieldByKey(key);

        //    return View(field ?? new Field() { FieldKey = key });
        //}

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(Field field)
        {
            field = Data.SaveField(field);

            return new JsonResult
            {
                Data = field,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}