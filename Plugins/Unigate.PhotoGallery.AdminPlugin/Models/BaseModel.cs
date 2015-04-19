using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Models
{
    public class BaseModel
    {
        public Guid ContentId { get; set; }
        public string Title { get; set; }
    }
}