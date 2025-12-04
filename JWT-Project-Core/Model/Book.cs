using JWT_Project_Core.Model.Base;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model
{
    public class Book : BaseEntity
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
        public ICollection<Order_Book> Order_Books { get; set; } = new List<Order_Book>();
    }
}
