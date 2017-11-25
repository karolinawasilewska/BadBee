using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumag.Core.MyResources;

namespace Lumag.Core.DAL
{
    [MetadataType(typeof(CrossesMetadata))]
    public partial class Crosses
    {

    }

    internal class CrossesMetadata
    {
        [Required]
        [Display(Name = "lumag_no", ResourceType = typeof(Resources))]
        public string LumagNumber { get; set; }
        [Required]
        [Display(Name = "cross_brand_name", ResourceType = typeof(Resources))]
        public string CrossBrandName { get; set; }
        [Required]
        [Display(Name = "cross_brand_number", ResourceType = typeof(Resources))]
        public string CrossBrandNumber { get; set; }
    }
}
