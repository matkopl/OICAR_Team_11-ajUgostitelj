using AutoMapper;
using WebApp.DTOs;
using WebApp.ViewModels;

namespace WebApp.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, ProductIndexViewModel>();
            CreateMap<CategoryDto, CategoryViewModel>();
            // Dodajte druge potrebne mape
        }
    }
}
