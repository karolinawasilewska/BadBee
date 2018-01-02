using BadBee.Core.MyResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BadBee.Core.Models
{
    [Serializable]
    public class CvlItem
    {
        public int Id { get; set; }
        [Display(Name = "brand", ResourceType = typeof(Resources))]
        public string Brand { get; set; }
        [Display(Name = "serie", ResourceType = typeof(Resources))]
        public string Serie { get; set; }
        [Display(Name = "model", ResourceType = typeof(Resources))]
        public string Model { get; set; }
        [Display(Name = "year", ResourceType = typeof(Resources))]
        public string Years { get; set; }
        [Display(Name = "date_from", ResourceType = typeof(Resources))]
        public string DateFrom { get; set; }
        [Display(Name = "date_to", ResourceType = typeof(Resources))]
        public string DateTo { get; set; }
        [Display(Name = "front_rear", ResourceType = typeof(Resources))]
        public string Fr { get; set; }
        [Display(Name = "wva_desc", ResourceType = typeof(Resources))]
        public string WvaDesc { get; set; }
        [Display(Name = "wva", ResourceType = typeof(Resources))]
        public string Wva { get; set; }
        [Display(Name = "BadBee_no", ResourceType = typeof(Resources))]
        public string BadBeeNumber { get; set; }
        [Display(Name = "size", ResourceType = typeof(Resources))]
        public string Size { get; set; }
        [Display(Name = "height", ResourceType = typeof(Resources))]
        public string Height { get; set; }
        [Display(Name = "width", ResourceType = typeof(Resources))]
        public string Width { get; set; }
        [Display(Name = "thickness", ResourceType = typeof(Resources))]
        public string Thickness { get; set; }
        [Display(Name = "brakesystem", ResourceType = typeof(Resources))]
        public string BrakeSystem { get; set; }
    }


}