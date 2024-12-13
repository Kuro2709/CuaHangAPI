namespace CuaHangAPI.Dtos
{
    public class AddProductDto
    {
        public required string ProductID { get; set; }
        public required string ProductName { get; set; }
        public required decimal Price { get; set; }
    }
}
