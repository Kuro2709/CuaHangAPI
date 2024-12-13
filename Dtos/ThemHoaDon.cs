namespace CuaHangAPI.Dtos
{
    public class ThemHoaDonDtos
    {
        public required string InvoiceID { get; set; }
        public required string CustomerID { get; set; }
        public required DateTime InvoiceDate { get; set; }
        public required ICollection<ThemChiTietHoaDonDto> InvoiceDetails { get; set; }
    }

    public class ThemChiTietHoaDonDto
    {
        public required string ProductID { get; set; }
        public required int Quantity { get; set; }
    }
}
