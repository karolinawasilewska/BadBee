using BadBee.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadBeeCatalog.Models
{
    public class CatalogContent
    {
        public PagedList.StaticPagedList<CvlItem> Items { get; set; }
        public List<CatalogContentTableHeader> TopHeaderItems { get; set; }
        public List<CatalogContentTableHeader> DetailsHeaderItems { get; set; }
    }
}