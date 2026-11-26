namespace Cairo_Cinema.API.DTOs.Request
{
    public class FilterMovieRequest
    {
        public decimal Price { get; set; }
        public int RateId { get; set; }
        public int GenreId { get; set; }
        public int CinemaId { get; set; }
        public int Page { get; set; } = 1;

    }
}
