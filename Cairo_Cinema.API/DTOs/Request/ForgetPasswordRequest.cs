namespace Cairo_Cinema.API.DTOs.Request
{
    public class ForgetPasswordRequest
    {
        public int Id {  get; set; }
        public string UserNameOrEmail { get; set; }
    }
}
