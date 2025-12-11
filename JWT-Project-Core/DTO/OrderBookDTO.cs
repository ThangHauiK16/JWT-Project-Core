using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.DTO
{
    public class OrderBookDTO
    {
        [Required]
        public string MaSach { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int SoLuong { get; set; }

        public string? TenSach { get; set; }
        public string? TacGia { get; set; }
        public double? Gia { get; set; }
        public string? HinhAnh { get; set; }
    }
}
