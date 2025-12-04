using JWT_Project_Core.Enum;
using JWT_Project_Core.Model.Base;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model.Human
{
    public class User : BaseEntity
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public EnumRole Role { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
