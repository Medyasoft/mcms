using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mcms.Sdk;

namespace Unigate.Plugins.Editable
{
    public class Field : UnigateBaseList
    {
        public string FieldKey { get; set; }
        public string Value { get; set; }
        //public string FieldType { get; set; }
        public int PageId { get; set; }
        public string Errors { get; set; }
    }
}