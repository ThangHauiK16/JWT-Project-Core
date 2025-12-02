using JWT_Project_Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.DTO
{
    public class HoaDonDTO
    {
        public Guid MaHoaDon { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public EnumStatus TrangThai { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public List<HoaDon_SachDTO> HoaDon_Saches { get; set; } = new List<HoaDon_SachDTO>();
    }
}
