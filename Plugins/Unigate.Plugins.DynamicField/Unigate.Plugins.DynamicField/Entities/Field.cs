using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mcms.Sdk;

namespace Unigate.Plugins.DynamicField.Entities
{
    public class Field : UnigateBaseList
    {
        public string FieldKey { get; set; }
        public string FieldValue { get; set; }
        public string FieldType { get; set; }
        public int PageId { get; set; }

        public override string ToString()
        {
            return this == null ? string.Empty : this.FieldValue;
        }
    }
}