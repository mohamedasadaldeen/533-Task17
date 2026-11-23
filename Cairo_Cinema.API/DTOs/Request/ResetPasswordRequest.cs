using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.DTOs.Request
{
    public class ResetPasswordRequest
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password),Compare(nameof(NewPassword))]
        public string ConfirmPassword{ get; set; }

        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
