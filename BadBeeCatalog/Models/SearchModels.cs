using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadBeeCatalog.Models
{
    public class SearchModel
    {
       public List<SearchColumnModel> Columns { get; set; }
    }

    public class SearchColumnModel
    {
        public List<SearchRowModel> Rows { get; set; }
    }

    public class SearchRowModel
    {
        public string Name { get; set; }
        public object CustomParams { get; set; }
    }
}