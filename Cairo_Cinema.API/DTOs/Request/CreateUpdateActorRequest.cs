using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.DTOs.Request
{
    public class CreateUpdateActorRequest
    {
        [MinLength(2)]
        public string Name { get; set; }
        public IFormFile? ImageFile { get; set; }

    }
}
