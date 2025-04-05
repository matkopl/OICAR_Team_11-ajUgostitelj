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
        }
    }
}
