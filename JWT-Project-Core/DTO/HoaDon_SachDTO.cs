using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.DTO
{
    public class HoaDon_SachDTO
    {
        [Required]
        public string MaSach { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int SoLuong { get; set; }
    }
}
