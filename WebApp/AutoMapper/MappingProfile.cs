using AutoMapper;
using WebAPI.DTOs;
using WebApp.ViewModels;

namespace WebApp.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, ProductViewModel>();
            CreateMap<CategoryDto, CategoryViewModel>();
            // Dodajte druge potrebne mape
        }
    }
}
