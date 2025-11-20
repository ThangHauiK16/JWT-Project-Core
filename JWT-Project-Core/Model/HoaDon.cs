using JWT_Project_Core.Model.Human;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Project_Core.Model
{
    public class HoaDon
    {
        [Key]
        public Guid MaHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public string? Username { get; set; }
        public User? User { get; set; }
        public ICollection<HoaDon_Sach> HoaDon_Saches { get; set; } = new List<HoaDon_Sach>();
    }
}
