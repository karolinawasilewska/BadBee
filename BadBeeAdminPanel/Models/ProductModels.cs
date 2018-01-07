using BadBee.Core.MyResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BadBeeAdminPanel.Models
{
    public class ProductModels
    {
        public int Id { get; set; }
        [Display(Name = "brand", ResourceType = typeof(Resources))]
        public string Brand { get; set; }
        [Display(Name = "type", ResourceType = typeof(Resources))]
        public string Type { get; set; }
        [Display(Name = "serie", ResourceType = typeof(Resources))]
        public string Serie { get; set; }
        [Display(Name = "model", ResourceType = typeof(Resources))]
        public string ModelName { get; set; }
        [Display(Name = "kw", ResourceType = typeof(Resources))]
        public string Kw { get; set; }
        [Display(Name = "km", ResourceType = typeof(Resources))]
        public string Km { get; set; }
        [Display(Name = "date_from_format", ResourceType = typeof(Resources))]
        public Nullable<System.DateTime> DateFrom { get; set; }
        [Display(Name = "date_to_format", ResourceType = typeof(Resources))]
        public Nullable<System.DateTime> DateTo { get; set; }
        [Display(Name = "front_rear", ResourceType = typeof(Resources))]
        public string Fr { get; set; }
        [Display(Name = "wva_desc", ResourceType = typeof(Resources))]
        public string WvaDesc { get; set; }
        [Display(Name = "wva", ResourceType = typeof(Resources))]
        public string Wva { get; set; }
        [Display(Name = "wva_det_qty", ResourceType = typeof(Resources))]
        public string WvaDetailsQty { get; set; }
        [Display(Name = "wva_det", ResourceType = typeof(Resources))]
        public string WvaDetails { get; set; }
        [Display(Name = "BadBee_no", ResourceType = typeof(Resources))]
        public string BadBeeNumber { get; set; }
        //[Display(Name = "size", ResourceType = typeof(Resources))]
        //public string Size { get; set; }
        [Display(Name = "height", ResourceType = typeof(Resources))]
        public decimal Height { get; set; }
        [Display(Name = "width", ResourceType = typeof(Resources))]
        public decimal Width { get; set; }
        [Display(Name = "thickness", ResourceType = typeof(Resources))]
        public decimal Thickness { get; set; }
        [Display(Name = "wedge", ResourceType = typeof(Resources))]
        public string Wedge { get; set; }
        [Display(Name = "drumdiameter", ResourceType = typeof(Resources))]
        public string DrumDiameter { get; set; }
        [Display(Name = "brakesystem", ResourceType = typeof(Resources))]
        public string BrakeSystem { get; set; }
        [Display(Name = "rivets_qtt", ResourceType = typeof(Resources))]
        public string RivetsQuantity { get; set; }
        [Display(Name = "rivets", ResourceType = typeof(Resources))]
        public string RivetsType { get; set; }
        [Display(Name = "product_name", ResourceType = typeof(Resources))]
        public string ProductName { get; set; }
        public Nullable<int> BrandId { get; set; }
        public Nullable<int> SerieId { get; set; }
        public Nullable<int> ModelId { get; set; }
        //public Nullable<int> KwId { get; set; }
        //public Nullable<int> KmId { get; set; }
        public Nullable<int> WvaId { get; set; }
        public Nullable<int> WvaDetailsId { get; set; }
        public Nullable<int> BadBeeNumberId { get; set; }
        public Nullable<int> ThicknessId { get; set; }
        public Nullable<int> RivetTypeId { get; set; }
        public Nullable<int> WidthId { get; set; }
        public Nullable<int> HeightId { get; set; }
        public Nullable<int> SystemId { get; set; }
        public Nullable<int> DrumDiameterId { get; set; }
    }
}