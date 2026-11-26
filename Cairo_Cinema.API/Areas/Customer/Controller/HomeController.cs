using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.API.Areas.Customer.Controller
{
    [Area(CD.CUSTOMER)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Movie> _movieRepository;
        public HomeController(ApplicationDbContext context, IRepository<Movie> movieRepository)
        {
            _context = context;
            _movieRepository = movieRepository;
        }
        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]FilterMovieRequest filter)
        {
            var movies = await _movieRepository.GetListAsync(includes: [m => m.Category, m => m.Actor, m => m.Cinema]);
            if (filter.Price > 0)
            {
                movies = movies.Where(m => m.Price.Equals(filter.Price));
            }

            if (filter.RateId > 0)
            {
                movies = movies.Where(m => m.Rate >= filter.RateId);
            }

            if (filter.GenreId > 0)
            {
                movies = movies.Where(m => m.CategoryId == filter.GenreId);
            }

            if (filter.CinemaId > 0)
            {
                movies = movies.Where(m => m.CinemaId == filter.CinemaId);
            }

            //Pagination
            var totalPages = (int)Math.Ceiling(movies.Count() / 4.0);
            var currentPage = filter.Page;
            movies = movies.Skip((filter.Page - 1) * 4).Take(4);

            return Ok(new ApiResponse<MovieResponse>
            {
                IsSuccess = true,
                Message = "Movies retrieved successfully",
                Data = new MovieResponse()
                {
                    Movie = movies.Adapt<List<MovieDtoResponse>>(),
                    TotalPages = totalPages,
                    CurrentPage = currentPage
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CinemaDetails(int id)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id , includes: [m => m.Category, m => m.Actor, m => m.Cinema]);
            if (movie == null) return NotFound();
            var relatedMovies = await _movieRepository.GetListAsync(includes: [m => m.Category, m => m.Actor, m => m.Cinema]);
            relatedMovies = relatedMovies.Where(m => m.Id != id).OrderBy(m => Guid.NewGuid()).Take(4);
            return Ok(new ApiResponse<MovieWithRelatedResponse>
            {
                IsSuccess = true,
                Message = "Movie retrieved successfully",
                Data = new MovieWithRelatedResponse()
                {
                    Movie = movie.Adapt<MovieDtoResponse>(),                         
                    RelatedMovies = relatedMovies.Adapt<List<MovieDtoResponse>>()
                }
            });
        }
    }
}
