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
    
    public partial class Dimension
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Dimension()
        {
            this.BadBee = new HashSet<BadBee>();
        }
    
        public int DimensionId { get; set; }
        public Nullable<int> WidthId { get; set; }
        public Nullable<int> HeightId { get; set; }
        public Nullable<int> ThicknessId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BadBee> BadBee { get; set; }
        public virtual Height Height { get; set; }
        public virtual Thickness Thickness { get; set; }
        public virtual Width Width { get; set; }
    }
}
