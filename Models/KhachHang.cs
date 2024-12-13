using System.ComponentModel.DataAnnotations;

namespace CuaHangAPI.Models
{
    public class ThongTinKhachHang
    {
        [Key]
        [StringLength(50)]
        public required string CustomerID { get; set; }

        [StringLength(255)]
        public required string CustomerName { get; set; }

        [StringLength(15)]
        public required string Phone { get; set; }
    }
}
