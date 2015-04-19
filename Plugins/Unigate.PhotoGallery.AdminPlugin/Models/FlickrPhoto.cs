using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Models
{
    public class FlickrPhoto
    {
        public string PhotoId { get; set; }
        public string ViewUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string Title { get; set; }
    }
}