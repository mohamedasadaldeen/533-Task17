using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        [MinLength(2)]
        public string Name { get; set; }
        public string? Image { get; set; } = "actor.png";

    }
}
