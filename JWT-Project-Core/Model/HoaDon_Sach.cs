using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model
{
    public class HoaDon_Sach
    {
        [Key]
        public Guid MaHoaDon { get; set; }
        public HoaDon? HoaDon { get; set; }

        public string? MaSach { get; set; }
        public Sach? Sach { get; set; }

        public int SoLuong { get; set; } 
    }
}
