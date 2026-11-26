namespace Cairo_Cinema.API.DTOs.Response
{
    public class MovieWithRelatedResponse
    {
    public MovieDtoResponse Movie { get; set; } = null!;
    public IEnumerable<MovieDtoResponse> RelatedMovies { get; set; } = new List<MovieDtoResponse>();
    }
}
