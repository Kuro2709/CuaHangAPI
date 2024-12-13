using System.ComponentModel.DataAnnotations;

namespace CuaHangAPI.Models
{
    public class ThongTinHoaDon
    {
        [Key]
        [StringLength(50)]
        public required string InvoiceID { get; set; }

        [StringLength(50)]
        public required string CustomerID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public required DateTime InvoiceDate { get; set; } = DateTime.Now;

        public required decimal TotalPrice { get; set; }

        public required virtual ThongTinKhachHang Customer { get; set; }
        public virtual ICollection<ThongTinChiTietHoaDon> InvoiceDetails { get; set; } = [];

        public void RecalculateTotalPrice(Func<string, decimal> getProductPrice)
        {
            TotalPrice = 0;
            foreach (var detail in InvoiceDetails)
            {
                detail.TotalPrice = detail.Quantity * getProductPrice(detail.ProductID);
                TotalPrice += detail.TotalPrice;
            }
        }
    }
}
