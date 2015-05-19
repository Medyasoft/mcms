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
using Unigate.Plugins.DynamicField.Entities;

namespace Unigate.Plugins.DynamicField
{
    public static class Extensions
    {
        public static IHtmlString Editable(this HtmlHelper htmlHelper, string fieldName, string fieldType = "text", bool pageBased = true)
        {
            var siteLanguageId = Page_Context.Current.Page.PageModel.SiteLanguage.ContentId;
            int pageId = Page_Context.Current.Page.PageModel.Id;

            Query query = UnigateObject.Query("DynamicField")
                .WhereEqualTo("FieldKey", fieldName)
                .WhereEqualTo("SiteLanguageId", siteLanguageId);

            if (pageBased)
            {
                query = query.WhereEqualTo("PageId", pageId);
            }

            var field = query
                .ToList<Field>()
                .FirstOrDefault();

            if (field != null)
                field.SiteLanguageId = siteLanguageId;

            var roleRepository = EngineContext.Current.Resolve<IRoleRepository>();
            var role = roleRepository.SelectRole("Admin");

            var currentUser = UserUtility.CurrentUser;
            string _value = field == null ? string.Empty : field.FieldValue;

            if (role != null && currentUser != null)
            {
                if (currentUser.UserRoleSites.Any(x => x.RoleId == role.Id))
                {
                    string divId = Guid.NewGuid().ToString();

                    return new HtmlString("<script>$( function() { Editable('" + divId + "','" + fieldName + "','" + ((field == null || string.IsNullOrEmpty(field.FieldType)) ? fieldType : field.FieldType) + "','" + siteLanguageId + "','" + (pageBased ? pageId : 0) + "')})</script><div id=\"" + divId + "\" class=\"editfield\">" + _value + "</div>");
                }
            }

            return new HtmlString(_value);


        }
    }
}