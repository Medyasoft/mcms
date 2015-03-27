using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiResponseFilter.Helpers
{
    public static class Extensions
    {
        public static bool IsList(this PropertyInfo prop)
        {
            string[] arrayTypes = new string[] { typeof(ICollection).Name, "Collection", "IEnumerable", "IList", "List" };
            foreach (var item in arrayTypes)
            {
                if (prop.PropertyType.Name.StartsWith(item))
                    return true;
            }

            return false;
        }

    }
}
