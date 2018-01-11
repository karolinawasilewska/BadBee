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
        [Required]
        public string Brand { get; set; }
        [Display(Name = "serie", ResourceType = typeof(Resources))]
        public string Serie { get; set; }
        [Display(Name = "model", ResourceType = typeof(Resources))]
        [Required]
        public string ModelName { get; set; }
        [Display(Name = "date_from_format", ResourceType = typeof(Resources))]
        public Nullable<int> DateFrom { get; set; }
        [Display(Name = "date_to_format", ResourceType = typeof(Resources))]
        public Nullable<int> DateTo { get; set; }
        [Display(Name = "front_rear", ResourceType = typeof(Resources))]
        [Required]
        public string Fr { get; set; }
        [Display(Name = "wva_desc", ResourceType = typeof(Resources))]
        public string WvaDesc { get; set; }
        [Display(Name = "wva", ResourceType = typeof(Resources))]
        [Required]
        public string Wva { get; set; }
        [Required]
        [Display(Name = "BadBee_no", ResourceType = typeof(Resources))]
        public string BadBeeNumber { get; set; }

        [Required]
        [Display(Name = "height", ResourceType = typeof(Resources))]
        public decimal Height { get; set; }
        [Required]
        [Display(Name = "width", ResourceType = typeof(Resources))]
        public decimal Width { get; set; }
        [Required]
        [Display(Name = "thickness", ResourceType = typeof(Resources))]
        public decimal Thickness { get; set; }
        [Required]
        [Display(Name = "brakesystem", ResourceType = typeof(Resources))]
        public string BrakeSystem { get; set; }
        public Nullable<int> BrandId { get; set; }
        public Nullable<int> SerieId { get; set; }
        public Nullable<int> ModelId { get; set; }
        public Nullable<int> WvaId { get; set; }
        public Nullable<int> BadBeeNumberId { get; set; }
        public Nullable<int> ThicknessId { get; set; }
        public Nullable<int> WidthId { get; set; }
        public Nullable<int> HeightId { get; set; }
        public Nullable<int> SystemId { get; set; }
    }
}