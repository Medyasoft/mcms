using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcms.Sdk;
using MCMS.Common.Models.ViewModels;
using MCMS.Common.Result;

namespace Unigate.Plugins.Editable.Dal
{
    public class Data
    {
        public static Field SaveField(Field field)
        {
            MDataSourceResult result;
            var _field = UnigateObject.Query("Editable")
                           .WhereEqualTo("FieldKey", field.FieldKey)
                           .WhereEqualTo("SiteLanguageId", field.SiteLanguageId)
                           .WhereEqualTo("PageId", field.PageId)
                           .ToList<Field>()
                           .FirstOrDefault();

            if (_field == null)
            {
                result = UnigateObject.Insert<Field>("Editable", field)
                         .Column("SiteLanguageId", field.SiteLanguageId)
                         .Execute();
                _field = field;
            }
            else
            {
                result = UnigateObject.Update("Editable")
                         .Column("Value", field.Value ?? "&nbsp;")
                         .WhereEqualTo("ContentId", _field.ContentId)
                         .Execute();
                _field.Value = field.Value;
            }
            if (result.ResultCode != ResultCode.Successfull)
                _field.Errors = result.ResultMessage;
            return _field;
        }

        public static Field GetField(string fieldName, Guid siteLanguageId, int pageId, string fieldType = "text", bool pageBased = true)
        {
            Query query = UnigateObject.Query("Editable")
                .WhereEqualTo("FieldKey", fieldName)
                .WhereEqualTo("SiteLanguageId", siteLanguageId);

            if (pageBased)
            {
                query = query.WhereEqualTo("PageId", pageId);
            }

            Field field = query
                .ToList<Field>()
                .FirstOrDefault();

            return field;
        }
    }
}
