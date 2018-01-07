using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(WidthMetadata))]
    public partial class Width
    {
       // public string ModelName { get; set; }
    }

    internal class WidthMetadata
    {
        [Display(Name = "width", ResourceType = typeof(MyResources.Resources))]
        [Required]
        public string Width1 { get; set; }
      
    }
}