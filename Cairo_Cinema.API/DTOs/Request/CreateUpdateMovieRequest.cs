using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cairo_Cinema.API.DTOs.Request
{
    public class CreateUpdateMovieRequest
    {
        public string Name { get; set; }
        [MaxLength(100)]
        public string? Description { get; set; }
        [Range(100, 500)]
        public decimal Price { get; set; }
        [Range(1, 5)]
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }

        public IFormFile? ImageFile { get; set; }

        public List<IFormFile>? MovieSubImages { get; set; }

        [ForeignKey(nameof(ActorId))]
        public int ActorId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CinemaId))]
        public int CinemaId { get; set; }
    }
}