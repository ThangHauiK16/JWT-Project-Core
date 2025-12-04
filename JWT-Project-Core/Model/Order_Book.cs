using JWT_Project_Core.Model.Base;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model
{
    public class Order_Book  
    {
        [Key]
        public Guid MaHoaDon { get; set; }
        public Order? HoaDon { get; set; }

        public string? MaSach { get; set; }
        public Book? Sach { get; set; }

        public int SoLuong { get; set; } 
    }
}
