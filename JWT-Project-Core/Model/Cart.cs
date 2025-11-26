using JWT_Project_Core.DTO;
using JWT_Project_Core.Model.Human;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; } = Guid.NewGuid();

        public string? Username { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem>? CartItems { get; set; }
    }
}
