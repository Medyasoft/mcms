using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mcms.Sdk;
using Mcms.UI.Framework;
using Mcms.UI.Framework.Repository;
using Mcms.UI.Framework.Utilities;
using MCMS.Common.Runtime;
using Unigate.Plugins.Editable.Auth;
using Unigate.Plugins.Editable.Configuration;
using Unigate.Plugins.Editable.Dal;

namespace Unigate.Plugins.Editable
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Değer için anahtar kelime</param>
        /// <param name="type">Editlemede kullanılacak tip</param>
        /// <param name="pageBased">Sayfa bazlı verimi kullanılacak</param>
        /// <param name="defaultValue">Değer boş olduğunda ekrana yazılacak varsayılan string</param>
        /// <returns></returns>
        public static IHtmlString Editable(this HtmlHelper htmlHelper, string key, string type = "text", bool pageBased = true, string defaultValue = "")
        {
            var siteLanguageId = Page_Context.Current.Page.PageModel.SiteLanguage.ContentId;
            int pageId = Page_Context.Current.Page.PageModel.Id;

            Field field = Data.GetField(key, siteLanguageId, pageId, type, pageBased);

            if (field != null)
                field.SiteLanguageId = siteLanguageId;

            var roleRepository = DependencyResolver.Current.GetService<IRoleRepository>();
            var role = roleRepository.SelectRole("Admin");
            var currentUser = UserUtility.CurrentUser;

            string _value = field == null ? string.Empty : field.Value;
            if (string.IsNullOrEmpty(_value)) _value = defaultValue;

            if (EditableAuthorization.Authorization())
            {
                if (currentUser.UserRoleSites.Any(x => x.RoleId == role.Id))
                {
                    string divId = Guid.NewGuid().ToString();
                    //sayfada js methodu olması gerekiyor
                    return new HtmlString("<script>$( function() { Editable('" + divId + "','" + key + "','" + type + "','" + siteLanguageId + "','" + (pageBased ? pageId : 0) + "')})</script><div id=\"" + divId + "\" class=\"editfield\">" + _value + "</div>");
                }
            }

            return new HtmlString(_value);
        }
    }
}