﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(HeightMetadata))]
    public partial class Height
    {
       // public string ModelName { get; set; }
    }

    internal class HeightMetadata
    {
        [Display(Name = "height", ResourceType = typeof(MyResources.Resources))]
        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal Height1 { get; set; }
      
    }
}