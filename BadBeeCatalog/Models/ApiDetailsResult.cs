using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BadBeeCatalog.Models
{
    [Serializable]
    public class ApiDetailsResult
    {
        public string Id { get; set; }
        public string BadBeeId { get; set; }
        public string Image { get; set; }
    }
}
