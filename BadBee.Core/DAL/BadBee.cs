//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BadBee.Core.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class BadBee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BadBee()
        {
            this.Item = new HashSet<Item>();
        }
    
        public int BadBeeId { get; set; }
        public string BadBeeNo { get; set; }
        public Nullable<int> WvaId { get; set; }
        public Nullable<int> SystemId { get; set; }
        public Nullable<int> DimensionId { get; set; }
        public string FR { get; set; }
        public Nullable<int> PictureId { get; set; }
    
        public virtual Dimension Dimension { get; set; }
        public virtual Picture Picture { get; set; }
        public virtual Systems Systems { get; set; }
        public virtual Wva Wva { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Item { get; set; }
    }
}
