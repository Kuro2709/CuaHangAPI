namespace CuaHangAPI.Dtos
{
    public class ChinhSuaHoaDonDto
    {
        public required string CustomerID { get; set; }
        public required DateTime InvoiceDate { get; set; }
        public required ICollection<ChinhSuaChiTietHoaDonDto> InvoiceDetails { get; set; }
    }

    public class ChinhSuaChiTietHoaDonDto
    {
        public required string ProductID { get; set; }
        public required int Quantity { get; set; }
    }
}
