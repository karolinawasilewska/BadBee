using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(BrandDbMetadata))]
    public partial class Brand
    {
       // public string ModelName { get; set; }
    }

    internal class BrandDbMetadata
    {
        [Display(Name = "brand", ResourceType = typeof(MyResources.Resources))]
        [Required]
        public string Name { get; set; }
      
    }
}