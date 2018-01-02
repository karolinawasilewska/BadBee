using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(SystemsMetadata))]
    public partial class Systems
    {
     
    }

    internal class SystemsMetadata
    {
        [Display(Name = "brakesystem", ResourceType = typeof(MyResources.Resources))]
        public string Abbreviation { get; set; }

    }
}