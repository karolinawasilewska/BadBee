﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lumag.Core.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class LumagEntities : DbContext
    {
        public LumagEntities()
            : base("name=LumagEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Brands> Brands { get; set; }
        public virtual DbSet<Crosses> Crosses { get; set; }
        public virtual DbSet<DrumDiameters> DrumDiameters { get; set; }
        public virtual DbSet<Heights> Heights { get; set; }
        public virtual DbSet<LumagNumbers> LumagNumbers { get; set; }
        public virtual DbSet<Models> Models { get; set; }
        public virtual DbSet<RivetTypes> RivetTypes { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<Systems> Systems { get; set; }
        public virtual DbSet<Thicknesses> Thicknesses { get; set; }
        public virtual DbSet<Widths> Widths { get; set; }
        public virtual DbSet<Wva> Wva { get; set; }
        public virtual DbSet<Years> Years { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Kms> Kms { get; set; }
        public virtual DbSet<Kws> Kws { get; set; }
        public virtual DbSet<Pictures> Pictures { get; set; }
        public virtual DbSet<Indicators> Indicators { get; set; }
        public virtual DbSet<Rivets> Rivets { get; set; }
        public virtual DbSet<ItemsDb> ItemsDb { get; set; }
        public virtual DbSet<WvaDetails> WvaDetails { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
    
        public virtual ObjectResult<string> GetKeywords(string keywordPart)
        {
            var keywordPartParameter = keywordPart != null ?
                new ObjectParameter("KeywordPart", keywordPart) :
                new ObjectParameter("KeywordPart", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("GetKeywords", keywordPartParameter);
        }
    }
}