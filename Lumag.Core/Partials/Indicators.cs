using Lumag.Core.MyResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumag.Core.DAL
{
    [MetadataType(typeof(IndicatorsMetadata))]
    public partial class Indicators
    {
    }

    internal class IndicatorsMetadata
    {
        public int Id { get; set; }
        [Display(Name = "lumag_no", ResourceType = typeof(Resources))]
        public string LumagNo { get; set; }
        [Display(Name = "description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
        [Display(Name = "length", ResourceType = typeof(Resources))]
        public string Length { get; set; }
        [Display(Name = "application", ResourceType = typeof(Resources))]
        public string Application { get; set; }
        [Display(Name = "qtt_in_kit", ResourceType = typeof(Resources))]
        public string Quantity { get; set; }
    }
}
