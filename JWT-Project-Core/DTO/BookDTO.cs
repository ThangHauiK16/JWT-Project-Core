using JWT_Project_Core.Model.Base;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.DTO
{
    public class BookDTO : BaseEntityDTO
    {
        [Required(ErrorMessage = "Mã sách là bắt buộc!")]
        [StringLength(50, ErrorMessage = "Mã sách không được vượt quá 50 ký tự.")]
        public string? MaSach { get; set; }

        [Required(ErrorMessage = "Tên sách là bắt buộc!")]
        [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự.")]
        public string? TenSach { get; set; }

        [StringLength(100, ErrorMessage = "Thể loại không được vượt quá 100 ký tự.")]
        public string? TheLoai { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn hoặc bằng 0.")]
        public double GiaNhap { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0.")]
        public double GiaBan { get; set; }

        [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự.")]
        public string? TenTacGia { get; set; }

        [StringLength(2000, ErrorMessage = "Nội dung sách không được vượt quá 2000 ký tự.")]
        public string? NoiDungSach { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int SoLuong { get; set; }

        public string? ImageUrl { get; set; }

    }
}
