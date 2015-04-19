using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Models
{
    public class PhotoViewModel : BaseContentModel
    {
        public string Photo { get; set; }
        public int OrderNo { get; set; }
    }
}