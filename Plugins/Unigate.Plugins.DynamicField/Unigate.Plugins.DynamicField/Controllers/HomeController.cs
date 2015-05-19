using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mcms.UI.Framework.PluginArea;
using Mcms.UI.Framework.PluginAttribute;

namespace Unigate.Plugins.DynamicField.Controllers
{
    [UIController(Name = "Dynamic Field")]
    public class HomeController : Controller
    {
        // GET: Home
        [UIAction(Name = "Dynamic Field")]
        public ActionResult Index()
        {
            string viewFile = PluginContext.Current.GetProperty("ViewFile");
            return View(viewFile);
        }


    }

    //public class CustomUIActionAttribute : UIActionAttribute
    //{
    //    override 
    //}
}