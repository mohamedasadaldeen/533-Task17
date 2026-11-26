namespace Cairo_Cinema.API.DTOs.Response
{
    public class MovieDtoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public string MainImage { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int ActorId { get; set; }
        public string? ActorName { get; set; }
        public int CinemaId { get; set; }
        public string? CinemaName { get; set; }
    }
}
