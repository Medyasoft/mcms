using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using WebApiResponseFilter.Helpers;

namespace WebApiResponseFilter.Helpers
{
    public class Utility
    {
        public static object CreateObject<T>(T t, List<string> properties)
        {
            if (t == null) return null;

            dynamic obj = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object>)obj;

            foreach (var property in properties)
            {
                var props = property.Split('.').ToList();
                var currentProp = props[0];

                List<string> listItems = null;
                NodeTypes nodeType = NodeTypes.Property;
                bool isList = false;
                if (currentProp.Contains("[") && currentProp.Contains("]"))
                {
                    listItems = currentProp.Split('[')[1].Replace("]", "").Split(';').ToList();
                    currentProp = currentProp.Split('[')[0];
                    isList = true;
                    nodeType = NodeTypes.List;
                }

                if (currentProp.Contains("(") && currentProp.Contains(")"))
                {
                    listItems = currentProp.Split('(')[1].Replace(")", "").Split(';').ToList();
                    currentProp = currentProp.Split('(')[0];
                    nodeType = NodeTypes.SubProperty;
                }

                var prop = t.GetType().GetProperty(currentProp);
                if (prop == null) continue;

                if (nodeType == NodeTypes.List)
                {
                    List<dynamic> dynamicList = new List<dynamic>();

                    foreach (var item in ((IList)prop.GetValue(t)))
                    {
                        var o = CreateObject(item, listItems);
                        dynamicList.Add(o);
                    }

                    dict[prop.Name] = dynamicList;
                }
                else if (nodeType == NodeTypes.SubProperty)
                {
                    var val = CreateObject(prop.GetValue(t), listItems);
                    dict[currentProp] = val;
                }
                else 
                {
                    var val = prop.GetValue(t);
                    dict[property] = val;
                }

                //if (isList)
                //{
                    
                //    //return dict;
                //}
                //else if (props.Count > 1)
                //{
                //    var subPropName = props[0];
                //    PropertyInfo subProp = t.GetType().GetProperty(subPropName);
                //    if (subProp == null) continue;

                //    string newProps = string.Join(".", props.GetRange(1, props.Count - 1));
                //    dict[prop.Name] = CreateObject(subProp.GetValue(t), new List<string>() { newProps });
                //}
                //else
                //{
                //    var val = prop.GetValue(t);
                //    dict[property] = val;
                //}
            }

            return dict;
        }

        public static List<string> GetFilterForMethod(string methodName)
        {
            string filePath = "~/App_Data/ApiFilters.txt";
            filePath = HostingEnvironment.MapPath(filePath);

            if (!File.Exists(filePath))
            {
                return null;
            }

            var lines = File.ReadAllLines(filePath).ToList().FirstOrDefault(x => x.StartsWith(methodName.ToLowerInvariant()));
            List<string> filters = null;
            if (lines != null)
            {
                filters = lines.Split(':')[1].Split(',').ToList();
            }

            return filters;
        }
    }
}
