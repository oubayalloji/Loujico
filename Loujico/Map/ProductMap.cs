using AutoMapper;
using Loujico.Models;

namespace Loujico.Map
{
    public class ProductMap:Profile
    {
        public ProductMap() 
        {
            CreateMap<AddProductModel, TbProduct>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ReverseMap();
        }
    }
}
