using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Mcms.Sdk;

namespace Mcms.Plugins.AdminSiteRedirect.Models
{

    public class SiteRedirectModel : UnigateBaseList
    {
        [DisplayName("Site")]
        public int PageId { get; set; }
        [DisplayName("Eski Adres")]
        public string LocalAddress { get; set; }
        [DisplayName("Yeni Adres")]
        public string RouteAddress { get; set; }
        [DisplayName("Parametreleri Aktar")]
        public bool TransferQuerystring { get; set; }
        [DisplayName("Geçicidir (301)")]
        public bool IsTemporary { get; set; }
        [DisplayName("Sıra No")]
        public int OrderId { get; set; }

        public string RouteValue { get; set; }
    }
}