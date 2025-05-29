using Application.DTO;
using AutoMapper;
using Domain.Entities;

namespace API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductRequestDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductResponseDto>();
        }

    }
}
