namespace Cairo_Cinema.API.DTOs.Request
{
    public class ChangePasswordRequest
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
