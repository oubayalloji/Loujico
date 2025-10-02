using AutoMapper;
using Loujico.Models;

namespace Loujico.Map
{
    public class Test : Profile
    {
        public Test()
        {

            CreateMap<CompanyCreateDto, Co_Company_Name>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // رح تعطيه إنت بالـ Controller
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastVisit, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Addresses, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyActivities, opt => opt.Ignore())
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyEmployees, opt => opt.Ignore());

            CreateMap<CompanyUpdateDto, Co_Company_Name>()
             .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
             .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
             .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) // رح تعطيه إنت بالـ Controller
             .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
             .ForMember(dest => dest.LastVisit, opt => opt.Ignore())
          
             .ForMember(dest => dest.Addresses, opt => opt.Ignore())
             .ForMember(dest => dest.CompanyActivities, opt => opt.Ignore())
             .ForMember(dest => dest.Contacts, opt => opt.Ignore())
             .ForMember(dest => dest.CompanyEmployees, opt => opt.Ignore());

        }
    }
}
