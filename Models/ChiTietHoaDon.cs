using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuaHangAPI.Models
{
    public class ThongTinChiTietHoaDon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceDetailID { get; set; }

        [StringLength(50)]
        public required string InvoiceID { get; set; }

        [StringLength(50)]
        public required string ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng nhập số lượng hợp lệ")]
        public required int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        [ForeignKey("ProductID")]
        public required virtual ThongTinSanPham Product { get; set; }

        [ForeignKey("InvoiceID")]
        public required virtual ThongTinHoaDon Invoice { get; set; }
    }
}
