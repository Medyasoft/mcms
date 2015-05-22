using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mcms.UI.Framework.PluginArea;
using Mcms.UI.Framework.PluginAttribute;

namespace Unigate.Plugins.Editable.Controllers
{
    [UIController(Name = "Düzenlenebilir Etiketler")]
    public class HomeController : Controller
    {
        // GET: Home
        [UIAction(Name = "Düzenlenebilir Etiketler")]
        public ActionResult Index()
        {
            string viewFile = PluginContext.Current.GetProperty("ViewFile");
            return View(viewFile);
        }


    }
}