namespace CuaHangAPI.Models
{
    public class User
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; } // e.g., "Admin" or "User"
    }

}
