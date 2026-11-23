using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.DTOs.Request
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
