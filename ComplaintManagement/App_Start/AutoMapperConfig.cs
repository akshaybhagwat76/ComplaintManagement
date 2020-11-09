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

                cfg.CreateMap<SubCategoryMasterVM, SubCategoryMaster>();
                cfg.CreateMap<SubCategoryMaster, SubCategoryMasterVM>();

                cfg.CreateMap<CommitteeMasterVM, CommitteeMaster>();
                cfg.CreateMap<CommitteeMaster, CommitteeMasterVM>();

                cfg.CreateMap<CompetencyMasterVM, CompetencyMaster>();
                cfg.CreateMap<CompetencyMaster, CompetencyMasterVM>();

                cfg.CreateMap<DesignationMasterVM, DesignationMaster>();
                cfg.CreateMap<DesignationMaster, DesignationMasterVM>();

                cfg.CreateMap<EntityMasterVM, EntityMaster>();
                cfg.CreateMap<EntityMaster, EntityMasterVM>();

                cfg.CreateMap<UserMasterVM, UserMaster>();
                cfg.CreateMap<UserMaster, UserMasterVM>();
            });
        }
    }
}