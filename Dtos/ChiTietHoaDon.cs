namespace CuaHangAPI.Dtos
{
    public class ChiTietHoaDonDto
    {
        public int InvoiceDetailID { get; set; }
        public required string InvoiceID { get; set; }
        public required string ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public required SanPhamDto Product { get; set; }
    }
}
