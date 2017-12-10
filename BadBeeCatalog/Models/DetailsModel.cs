using BadBee.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadBeeCatalog.Models
{
    public class DetailsModel
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Serie { get; set; }
        public string Models { get; set; }
        public string Kw { get; set; }
        public string Km { get; set; }
        public string Years { get; set; }
        public string Fr { get; set; }
        public string WvaDesc { get; set; }
        public string Wva { get; set; }
        public string WvaDetailsQty { get; set; }
        public string WvaDetails { get; set; }
        public string BadBeeNumber { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Thickness { get; set; }
        public string Wedge { get; set; }
        public string DrumDiameter { get; set; }
        public string System { get; set; }
        public string RivetsQuantity { get; set; }
        public string RivetsType { get; set; }
        public string Schema1 { get; set; }
        public string Schema2 { get; set; }
        public string Schema3 { get; set; }
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public int? PictureId { get; set; }
        public string Type { get; set; }


        public CvlItem Obj { get; set; }
        public string[] Pictures { get; set; }
        public string Picture { get; set; }
    }
}