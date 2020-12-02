using AutoMapper;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;

namespace ComplaintManagement.App_Start
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CategoryMasterVM, CategoryMaster>();
                cfg.CreateMap<CategoryMaster, CategoryMasterVM>();

                cfg.CreateMap<CategoryMasterHistoryVM, CategoryMasters_History>();
                cfg.CreateMap<CategoryMasters_History, CategoryMasterHistoryVM>();

                cfg.CreateMap<SubCategoryMasterVM, SubCategoryMaster>();
                cfg.CreateMap<SubCategoryMaster, SubCategoryMasterVM>();

                cfg.CreateMap<SubCategoryMasterHistoryVM, SubCategoryMasters_History>();
                cfg.CreateMap<SubCategoryMasters_History, SubCategoryMasterHistoryVM>();

                cfg.CreateMap<CommitteeMasterVM, CommitteeMaster>();
                cfg.CreateMap<CommitteeMaster, CommitteeMasterVM>();

                cfg.CreateMap<CommitteeMasterHistoryVM, CommitteeMasters_History>();
                cfg.CreateMap<CommitteeMasters_History, CommitteeMasterHistoryVM>();

                cfg.CreateMap<CompetencyMasterVM, CompetencyMaster>();
                cfg.CreateMap<CompetencyMaster, CompetencyMasterVM>();

                cfg.CreateMap<CompetencyMasterHistoryVM, CompetencyMasters_History>();
                cfg.CreateMap<CompetencyMasters_History, CompetencyMasterHistoryVM>();

                cfg.CreateMap<DesignationMasterVM, DesignationMaster>();
                cfg.CreateMap<DesignationMaster, DesignationMasterVM>();

                cfg.CreateMap<DesignationMasterHistoryVM, DesignationMasters_History>();
                cfg.CreateMap<DesignationMasters_History, DesignationMasterHistoryVM>();

                cfg.CreateMap<EntityMasterVM, EntityMaster>();
                cfg.CreateMap<EntityMaster, EntityMasterVM>();

                cfg.CreateMap<EntityMasterHistoryVM, EntityMasters_History>();
                cfg.CreateMap<EntityMasters_History, EntityMasterHistoryVM>();

                cfg.CreateMap<LocationMasterVM, LocationMaster>();
                cfg.CreateMap<LocationMaster, LocationMasterVM>();

                cfg.CreateMap<LocationMasterHistoryVM, LocationMasters_History>();
                cfg.CreateMap<LocationMasters_History, LocationMasterHistoryVM>();

                cfg.CreateMap<RegionMasterVM, RegionMaster>();
                cfg.CreateMap<RegionMaster, RegionMasterVM>();

                cfg.CreateMap<RegionMasterHistoryVM, RegionMasters_History>();
                cfg.CreateMap<RegionMasters_History, RegionMasterHistoryVM>();

                cfg.CreateMap<SBUMasterVM, SBUMaster>();
                cfg.CreateMap<SBUMaster, SBUMasterVM>();

                cfg.CreateMap<SBUMasterHistoryVM, SBUMasters_History>();
                cfg.CreateMap<SBUMasters_History, SBUMasterHistoryVM>();

                cfg.CreateMap<SubSBUMasterVM, SubSBUMaster>();
                cfg.CreateMap<SubSBUMaster, SubSBUMasterVM>();

                cfg.CreateMap<SubSBUMasterHistoryVM, SubSBUMasters_History>();
                cfg.CreateMap<SubSBUMasters_History, SubSBUMasterHistoryVM>();

                cfg.CreateMap<UserMasterVM, UserMaster>();
                cfg.CreateMap<UserMaster, UserMasterVM>();

                cfg.CreateMap<UserMasterHistoryVM, UserMasters_History>();
                cfg.CreateMap<UserMasters_History, UserMasterHistoryVM>();

                cfg.CreateMap<LOSMasterVM, LOSMaster>();
                cfg.CreateMap<LOSMaster, LOSMasterVM>();

                cfg.CreateMap<LOSMasterHistoryVM, LOSMasters_History>();
                cfg.CreateMap<LOSMasters_History, LOSMasterHistoryVM>();

                cfg.CreateMap<RoleMasterVM, RoleMaster>();
                cfg.CreateMap<RoleMaster, RoleMasterVM>();

                cfg.CreateMap<RoleMasterHistoryVM, RoleMasters_History>();
                cfg.CreateMap<RoleMasters_History, RoleMasterHistoryVM>();

                cfg.CreateMap<EmployeeCompliant_oneMasterVM, EmployeeComplaintMaster>();
                cfg.CreateMap<EmployeeComplaintMaster, EmployeeCompliant_oneMasterVM>();
            });
        }
    }
}