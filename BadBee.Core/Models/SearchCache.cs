using BadBee.Core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadBee.Core.Models
{
    public class SearchCache
    {
        public List<ItemsDb> GetListResult { get; set; }
        public string SearchKey { get; set; }
    }
}