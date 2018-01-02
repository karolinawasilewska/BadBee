using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(ModelMetadata))]
    public partial class Model
    {
    }

    internal class ModelMetadata
    {
        [Display(Name = "model", ResourceType = typeof(MyResources.Resources))]
        public string Name { get; set; }
      
    }
}