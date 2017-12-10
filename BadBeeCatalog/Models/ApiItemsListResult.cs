using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BadBee.Core.Models;

namespace BadBeeCatalog.Models
{
    [Serializable]
    public class ApiItemsListResult
    {
        public IList<CvlItem> Items { get; set; }
        public int ItemsCount { get; set; }
    }
}
