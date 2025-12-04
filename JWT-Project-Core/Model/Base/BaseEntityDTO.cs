namespace JWT_Project_Core.Model.Base
{
    public abstract class BaseEntityDTO
    {
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
