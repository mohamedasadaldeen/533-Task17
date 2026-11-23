namespace Cairo_Cinema.API.DTOs.Request
{
    public class ResendEmailConfirmationRequest
    {
        public int Id { get; set; }
        public string UserNameOrEmail { get; set; }
    }
}
