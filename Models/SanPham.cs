using System.ComponentModel.DataAnnotations;

namespace CuaHangAPI.Models
{
    public class ThongTinSanPham
    {
        [Key]
        [StringLength(50)]
        public required string ProductID { get; set; }

        [StringLength(255)]
        public required string ProductName { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "100000000", ErrorMessage = "Giá trị quá lớn hoặc quá nhỏ, vui lòng nhập giá trị hợp lệ")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Giá thành chỉ được chứa số và tối đa hai chữ số thập phân")]
        public required decimal Price { get; set; }
    }
}
