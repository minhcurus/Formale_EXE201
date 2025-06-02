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
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductResponseDto>();

            //User
            CreateMap<UserAccount, UserResponse>();
            CreateMap<UserDTO, UserAccount>();
            CreateMap<UserResponse, UserDTO>()
                 .ForMember(dest => dest.Image_User, opt => opt.Ignore())
                 .ForMember(dest => dest.Background_Image, opt => opt.Ignore());


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
