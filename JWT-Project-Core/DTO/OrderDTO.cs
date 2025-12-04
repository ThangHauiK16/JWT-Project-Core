using JWT_Project_Core.Enum;
using JWT_Project_Core.Model.Base;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.DTO
{
    public class OrderDTO : BaseEntityDTO
    {
        public Guid MaHoaDon { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public EnumStatus TrangThai { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public List<Order_BookDTO> Order_Books { get; set; } = new List<Order_BookDTO>();
    }
}
