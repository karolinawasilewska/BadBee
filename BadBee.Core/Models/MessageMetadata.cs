using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadBee.Core.DAL
{
   
    public class MessageMetadata
    {
        [StringLength(5)]
        [Display(Name = "MessageFrom")]
        public string MessageFrom { get; set; }
        [Required]
        public string Content { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
