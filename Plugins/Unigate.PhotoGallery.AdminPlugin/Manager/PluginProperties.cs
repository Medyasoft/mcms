using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Manager
{
    public static class PluginProperties
    {
        static string PluginName_ = string.Empty;
        public static string PluginName
        {
            get {
                if(string.IsNullOrEmpty(PluginName_))
                {
                    PluginName_ = Assembly.GetCallingAssembly().GetName().Name;
                }

                return PluginName_;
            }
        }
    }
}