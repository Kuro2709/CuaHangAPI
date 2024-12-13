namespace CuaHangAPI.Dtos
{
    public class HoaDonDto
    {
        public required string InvoiceID { get; set; }
        public required string CustomerID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalPrice { get; set; }
        public required KhachHangDto Customer { get; set; }
        public required ICollection<ChiTietHoaDonDto> ChiTietHoaDon { get; set; }
    }
}
