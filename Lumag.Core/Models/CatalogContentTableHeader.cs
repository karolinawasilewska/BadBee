using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lumag.Core.Models
{
    public class CatalogContentTableHeader
    {
        public string ID { get; set; }
        public string DataTarget { get; set; }
        public string OnClickMethod { get; set; }
        public string PictureSrc { get; set; }
        public string Description { get; set; }
        public bool IsFilterSelected { get; set; }
        public string CssClass { get; set; }
        public string DataToggle { get; set; }
        public string Colspan { get; set; }
        public string CssStyle { get; set; }
        public CatalogContentTableHeaderType Type { get; set; }
        public bool HasAnyElements { get; set; }
        public int? Index { get; set; }
    }

    public enum CatalogContentTableHeaderType
    {
        TopEmpty,
        TopFilter,
        DetailsEmpty,
        DetailsFilter
    }
}