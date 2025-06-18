using Application.DTO;
using AutoMapper;
using Domain.Entities;

namespace API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Product
            CreateMap<ProductRequestDto, Product>();
            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.Brand, opt => opt.MapFrom(s => s.Brand.BrandName))
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.CategoryName))
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ColorName))
                .ForMember(d => d.Material, opt => opt.MapFrom(s => s.Material.MaterialName))
                .ForMember(d => d.Style, opt => opt.MapFrom(s => s.Style.StyleName))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.TypeName));

            //User
            CreateMap<UserAccount, UserResponse>();
            CreateMap<UserDTO, UserAccount>();
            CreateMap<UserResponse, UserDTO>()
                 .ForMember(dest => dest.imageUser, opt => opt.Ignore())
                 .ForMember(dest => dest.imageBackground, opt => opt.Ignore());


            //Payment
            CreateMap<Payment, PaymentDTO>();

            //Order
            CreateMap<OrderDTO, Order>();
            CreateMap<Order, OrderDTO>();

            //PremiunPackage
            CreateMap<PremiumPackage, PremiunPackageDTO>();

        }

    }
}
