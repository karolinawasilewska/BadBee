using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(ThicknessMetadata))]
    public partial class Thickness
    {
       // public string ModelName { get; set; }
    }

    internal class ThicknessMetadata
    {
        [Display(Name = "thickness", ResourceType = typeof(MyResources.Resources))]
        [Required]
        public string Thickness1 { get; set; }
      
    }
}