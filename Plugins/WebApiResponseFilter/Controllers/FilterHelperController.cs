using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiResponseFilter.Controllers
{
    public class FilterHelperController : ApiController
    {
        /// <summary>
        /// Sistemdeki tüm Api Methodlarını verir.
        /// </summary>
        /// <returns></returns>
        public List<string> GetMethods()
        {
            List<string> methodNames = new List<string>();

            Assembly assemblies = Assembly.GetExecutingAssembly();
            var actions = assemblies.GetTypes()
                .Where(type => typeof(ApiController).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)));

            foreach (var item in actions)
            {
                methodNames.Add(item.Name);
            }

            return methodNames;
        }

    }
}
