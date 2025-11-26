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
            CreateMap<RegisterDTO, User>();
            CreateMap<Sach, SachDTO>().ReverseMap();
            CreateMap<HoaDonDTO, HoaDon>().ReverseMap();
            CreateMap<HoaDon_SachDTO, HoaDon_Sach>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();
            CreateMap<Cart, CartDTO>()
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.CartItems))
                .ReverseMap()
                .ForMember(dest => dest.CartItems,
                           opt => opt.MapFrom(src => src.Items));
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
