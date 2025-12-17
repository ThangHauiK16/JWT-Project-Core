using AutoMapper;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Model;
using JWT_Project_Core.Model.Human;

namespace JWT_Project_Core.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
         
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt.AddHours(7)))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.AddHours(7) : (DateTime?)null));

            
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore()) 
                .ForMember(dest => dest.Orders, opt => opt.Ignore()); 
          
            CreateMap<Book, BookDTO>()
               .ForMember(dest => dest.CreatedAt,
                   opt => opt.MapFrom(src => src.CreatedAt.AddHours(7)))
               .ForMember(dest => dest.UpdatedAt,
                   opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.AddHours(7) : (DateTime?)null));

           
            CreateMap<BookDTO, Book>()
               .ForMember(dest => dest.MaSach, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());


            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt.AddHours(7)))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.AddHours(7) : (DateTime?)null))
                .ForMember(dest => dest.Order_Books,
                    opt => opt.MapFrom(src => src.OrderBooks)); // thêm map chi tiết

            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.OrderBooks,
                    opt => opt.MapFrom(src => src.Order_Books)); // thêm map chi tiết


            //CreateMap<OrderBookDTO, OrderBook>().ReverseMap();

            CreateMap<OrderBook, OrderBookDTO>()
               .ForMember(dest => dest.TenSach, opt => opt.MapFrom(src => src.Sach != null ? src.Sach.TenSach : null))
               .ForMember(dest => dest.TacGia, opt => opt.MapFrom(src => src.Sach != null ? src.Sach.TenTacGia : null))
                .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.Sach != null ? src.Sach.GiaBan : 0))
               .ForMember(dest => dest.HinhAnh, opt => opt.MapFrom(src => src.Sach != null ? src.Sach.ImageUrl : null));

            CreateMap<OrderBookDTO, OrderBook>()
                .ForMember(dest => dest.Sach, opt => opt.Ignore());

            CreateMap<CartItem, CartItemDTO>().ReverseMap();
            CreateMap<Cart, CartDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
                .ReverseMap()
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.Items));
        }
    }
}