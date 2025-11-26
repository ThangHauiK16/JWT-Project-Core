using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Project_Core.Model
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("Cart")]
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }

        [ForeignKey("Sach")]
        public string? MaSach { get; set; }
        public Sach? Sach { get; set; }

        public int SoLuong { get; set; }
    }
}
