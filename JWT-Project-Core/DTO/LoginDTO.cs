namespace JWT_Project_Core.DTO
{
    public class LoginDTO
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? Password { get; set; }
    }
}
