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
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.TypeName))
                .ForMember(dest => dest.IsSystemCreated, opt => opt.MapFrom(src => src.IsSystemCreated)); ;

            //User
            CreateMap<UserAccount, UserResponse>();
            CreateMap<UserDTO, UserAccount>();
            CreateMap<UserAccount, UserUpdateResponse>()
                .ForMember(dest => dest.imageUser, opt => opt.MapFrom(src => src.Image_User))
                .ForMember(dest => dest.imageBackground, opt => opt.MapFrom(src => src.Background_Image));

            CreateMap<UserAccount, UserDTO>();
            CreateMap<UserResponse, UserDTO>()
                 .ForMember(dest => dest.imageUser, opt => opt.Ignore())
                 .ForMember(dest => dest.imageBackground, opt => opt.Ignore());


            //Payment
            CreateMap<Payment, PaymentDTO>();
            CreateMap<Payment, PaymentPackageResponse>();
            CreateMap<Payment, PaymentAllDTO>();

            //Order
            CreateMap<OrderItem, OrderItemDTO>();
            CreateMap<OrderDTO, Order>();
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));


            //PremiunPackage
            CreateMap<PremiumPackage, PremiunPackageDTO>();

            //Feedback
            CreateMap<FeedbackRequestDto, Feedback>();
            CreateMap<Feedback, FeedbackDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                 .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        }

    }
}
