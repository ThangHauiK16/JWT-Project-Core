using JWT_Project_Core.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Project_Core.Model
{
    public class OrderBook  
    {
        [Key]
        public Guid MaHoaDon { get; set; }
        public Order? HoaDon { get; set; }

        public string? MaSach { get; set; }
        public Book? Sach { get; set; }

        public int SoLuong { get; set; } 
    }
}
