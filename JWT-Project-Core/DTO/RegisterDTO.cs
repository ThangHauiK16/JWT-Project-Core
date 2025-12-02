using JWT_Project_Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project_Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Username is required!")]
        [MaxLength(50, ErrorMessage = "Username must be at most 50 characters!")]
        public string Username { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password is required!")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()\-_=+]).{8,}$",
            ErrorMessage = "Password must have at least 8 characters, including uppercase, lowercase, number and special character."
        )]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email must be at most 100 characters!")]
        public string Email { get; set; } = string.Empty;

    }
}
