using Lumag.Core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lumag.Core.Models
{
    public class SearchCache
    {
        public List<ItemsDb> GetListResult { get; set; }
        public string SearchKey { get; set; }
    }
}