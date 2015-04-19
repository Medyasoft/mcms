using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Models
{
    public class BaseContentModel : BaseModel
    {
        public int Id { get; set; }
        public Guid SiteLanguageId { get; set; }
        public string Slug { get; set; }
    }
}