﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComplaintManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_A6A061_complaintuserEntities : DbContext
    {
        public DB_A6A061_complaintuserEntities()
            : base("name=DB_A6A061_complaintuserEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CategoryMaster> CategoryMasters { get; set; }
        public virtual DbSet<CategoryMasters_History> CategoryMasters_History { get; set; }
        public virtual DbSet<CommitteeMaster> CommitteeMasters { get; set; }
        public virtual DbSet<CommitteeMasters_History> CommitteeMasters_History { get; set; }
        public virtual DbSet<CompetencyMaster> CompetencyMasters { get; set; }
        public virtual DbSet<CompetencyMasters_History> CompetencyMasters_History { get; set; }
        public virtual DbSet<DesignationMaster> DesignationMasters { get; set; }
        public virtual DbSet<DesignationMasters_History> DesignationMasters_History { get; set; }
        public virtual DbSet<EmployeeComplaintMaster> EmployeeComplaintMasters { get; set; }
        public virtual DbSet<EmployeeComplaintMastersHistory> EmployeeComplaintMastersHistories { get; set; }
        public virtual DbSet<EntityMaster> EntityMasters { get; set; }
        public virtual DbSet<EntityMasters_History> EntityMasters_History { get; set; }
        public virtual DbSet<LocationMaster> LocationMasters { get; set; }
        public virtual DbSet<LocationMasters_History> LocationMasters_History { get; set; }
        public virtual DbSet<LOSMaster> LOSMasters { get; set; }
        public virtual DbSet<LOSMasters_History> LOSMasters_History { get; set; }
        public virtual DbSet<RegionMaster> RegionMasters { get; set; }
        public virtual DbSet<RegionMasters_History> RegionMasters_History { get; set; }
        public virtual DbSet<RoleMaster> RoleMasters { get; set; }
        public virtual DbSet<RoleMasters_History> RoleMasters_History { get; set; }
        public virtual DbSet<SBUMaster> SBUMasters { get; set; }
        public virtual DbSet<SBUMasters_History> SBUMasters_History { get; set; }
        public virtual DbSet<SubCategoryMaster> SubCategoryMasters { get; set; }
        public virtual DbSet<SubCategoryMasters_History> SubCategoryMasters_History { get; set; }
        public virtual DbSet<SubSBUMaster> SubSBUMasters { get; set; }
        public virtual DbSet<SubSBUMasters_History> SubSBUMasters_History { get; set; }
        public virtual DbSet<UserMaster> UserMasters { get; set; }
        public virtual DbSet<UserMasters_History> UserMasters_History { get; set; }
        public virtual DbSet<EmployeeComplaintHistory> EmployeeComplaintHistories { get; set; }
    }
}
