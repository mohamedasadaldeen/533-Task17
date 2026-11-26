namespace Cairo_Cinema.API.DTOs.Response
{
    public class MovieResponse
    {
        //public IEnumerable<Movie> Movie { get; set; }
        public IEnumerable<MovieDtoResponse> Movie { get; set; } = new List<MovieDtoResponse>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
