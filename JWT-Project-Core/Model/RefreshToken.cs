namespace JWT_Project_Core.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
    }
}
