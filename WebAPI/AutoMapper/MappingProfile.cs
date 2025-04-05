using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Table, TableDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, ChangePasswordDto>().ReverseMap();
        }
    }
}
