using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiResponseFilter.Helpers;

namespace WebApiResponseFilter.ApiFilter
{
    public class Events
    {
        public static void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            if (objectContent != null)
            {
                string actionName = actionExecutedContext.Request.RequestUri.AbsolutePath;
                List<string> propNameForMethod = Utility.GetFilterForMethod(actionName);

                var headers = actionExecutedContext.Request.Headers;
                if (headers.Contains("filter"))
                {
                    string token = headers.GetValues("filter").First();
                    if (!string.IsNullOrEmpty(token))
                    {
                        propNameForMethod = token.Split(',').ToList();
                    }
                }
                else return;

                if (propNameForMethod == null) return;

                Type type = objectContent.ObjectType;
                object value = objectContent.Value;

                if (type.Name.StartsWith("List"))
                {
                    List<dynamic> dynamicList = new List<dynamic>();
                    foreach (var item in (IList)value)
                    {
                        var dict = Utility.CreateObject(item, propNameForMethod);
                        dynamicList.Add(dict);
                    }

                    objectContent.Value = dynamicList;
                }
                else
                {
                    objectContent.Value = Utility.CreateObject(value, propNameForMethod);
                }
            }

            //objectContent.Value = actionExecutedContext.Request.RequestUri.AbsolutePath;
        }

        public static void OnActionExecuting(HttpActionContext actionContext)
        {
 
        }
    }
}
