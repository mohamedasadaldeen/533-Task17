using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.DTOs.Response
{
    public class ApplicationUserResponse
    {
        public string? Name { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }  
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}