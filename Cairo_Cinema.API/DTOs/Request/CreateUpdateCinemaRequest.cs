namespace Cairo_Cinema.API.DTOs.Request
{
    public class CreateUpdateCinemaRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
