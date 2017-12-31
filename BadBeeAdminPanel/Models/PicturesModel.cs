using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BadBeeAdminPanel.Models
{
    public class PicturesModel
    {
        //public List<string> NamesList { get; set; }
        public StaticPagedList<string> Items { get; set; }
        

        // public string Caption { get; set; }
        //  public string Type { get; set; }
    }
}