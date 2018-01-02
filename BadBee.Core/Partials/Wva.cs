using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(WvaMetadata))]
    public partial class Wva
    {
    }

    internal class WvaMetadata
    {
        [Display(Name = "wva", ResourceType = typeof(MyResources.Resources))]
        public string WvaNo { get; set; }
        [Display(Name = "description", ResourceType = typeof(MyResources.Resources))]
        public string Description { get; set; }

    }
}