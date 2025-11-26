namespace JWT_Project_Core.DTO
{
    public class CartDTO
    {
        public Guid CartId { get; set; }
        public string? Username { get; set; }
        public List<CartItemDTO>? Items { get; set; }
    }
}
