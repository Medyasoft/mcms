using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Manager
{
    public class Settings
    {
        static Settings current_ = null;
        public static Settings Current
        {
            get
            {
                if (current_ == null)
                {
                    current_ = Read();
                }

                return current_;
            }
        }
        public string AlbumListTableName { get; set; }
        public string PhotoListTableName { get; set; }
        public ListTableField PhotoListTableTitleField { get; set; }
        public ListTableField PhotoListTablePhotoField { get; set; }
        public ListTableField PhotoListTableThumbnailField { get; set; }
        public ListTableField PhotoListTableOrderNoField { get; set; }
        public ListTableField PhotoListTableAlbumRelationField { get; set; }
        public PhotoSize PhotoSize { get; set; }
        public PhotoSize ThumbnailPhotoSize { get; set; }
        public string FlickrAPIKey { get; set; }
        public string FlickrUserName { get; set; }
        public static Settings Read()
        {
            Settings retVal = null;
            string jsonfile = string.Format(@"{0}Modules\Plugins\{1}\settings.json", HttpContext.Current.Server.MapPath("~"), PluginProperties.PluginName);

            try
            {
                using (StreamReader re = new StreamReader(jsonfile))
                {
                    using (JsonTextReader reader = new JsonTextReader(re))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        retVal = serializer.Deserialize<Settings>(reader);
                    }
                }
            }
            catch (Exception)
            { }

            return retVal;
        }
    }

    public class ListTableField
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int? MaxLenght { get; set; }
        public bool IsReguired { get; set; }
        public object DefaultValue { get; set; }
    }

    public class PhotoSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}