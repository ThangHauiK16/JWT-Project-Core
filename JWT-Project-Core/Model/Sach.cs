using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model
{
    public class Sach
    {
        [Key]
        public string? MaSach { get; set; }
        public string? TenSach { get; set; }
        public string? TheLoai { get; set; }
        public double GiaNhap { get; set; }
        public double GiaBan { get; set; }
        public string? TenTacGia { get; set; }
        public string?  NoiDungSach { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<HoaDon_Sach> HoaDon_Saches { get; set; } = new List<HoaDon_Sach>();
    }
}
