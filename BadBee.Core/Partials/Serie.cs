using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(SerieMetadata))]
    public partial class Serie
    {
    }

    internal class SerieMetadata
    {
        [Display(Name = "serie", ResourceType = typeof(MyResources.Resources))]
        public string Name { get; set; }
      
    }
}