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
            });
        }
    }
}