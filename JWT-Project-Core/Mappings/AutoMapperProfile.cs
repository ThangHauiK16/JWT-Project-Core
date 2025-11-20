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

        }
    }
}
