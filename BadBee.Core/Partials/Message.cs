using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BadBee.Core.MyResources;

namespace BadBee.Core.DAL
{
    [MetadataType(typeof(MessageMetadata))]
    public partial class Message
    {
       // public string ModelName { get; set; }
    }

    internal class MessageMetadata
    {
        public int Id { get; set; }
        [Display(Name = "E-mail")]
        public string MessageFrom { get; set; }
        [Display(Name = "Treść")]
        public string Content { get; set; }
        [Display(Name = "Przeczytana")]
        public Nullable<bool> IsRead { get; set; }
        [Display(Name = "Data otrzymania")]
        public Nullable<System.DateTime> InsertDate { get; set; }
        [Display(Name = "Imię i nazwisko")]
        public string Name { get; set; }

    }
}