using JWT_Project_Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.Model.Human
{
    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public EnumRole Role { get; set; }
        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
