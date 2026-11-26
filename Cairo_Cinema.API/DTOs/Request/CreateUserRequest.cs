using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.DTOs.Request
{
    public class CreateUserRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Role { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
