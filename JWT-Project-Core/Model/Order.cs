using JWT_Project_Core.Enum;
using JWT_Project_Core.Model.Base;
using JWT_Project_Core.Model.Human;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Project_Core.Model
{
    public class Order : BaseEntity
    {
        [Key]
        public Guid MaHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public string? Username { get; set; }
        public EnumStatus TrangThai { get; set; }
        public User? User { get; set; }
        public ICollection<OrderBook> OrderBooks { get; set; } = new List<OrderBook>();
    }
}
