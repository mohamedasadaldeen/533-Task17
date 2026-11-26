using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.API.Areas.Admin.Controller
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN},{CD.EMPLOYEE}")]
    public class MovieController : ControllerBase
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IMovieSubImageRepository _movieSubImageRepository;
        public MovieController(
            IRepository<Movie> movieRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Actor> actorRepository,
            IMovieSubImageRepository movieSubImageRepository
            )
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieSubImageRepository = movieSubImageRepository;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterMovieRequest filter)
        {
            var movies = await _movieRepository.GetListAsync(includes: [m => m.Actor, m => m.Cinema, m => m.Category]);
            //filter
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
                Message = "Movies retrieved successfully.",
                Data = new MovieResponse()
                {
                    Movie = movies.Adapt<List<MovieDtoResponse>>(),
                    TotalPages = totalPages,
                    CurrentPage = currentPage
                }
            });
        }

        //GetOne
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var movie = await _movieRepository.GetOneAsync(c => c.Id == id);

            if (movie == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Movie not found.",
                    Data = null
                });
            }
            return Ok(new ApiResponse<Movie>
            {
                IsSuccess = true,
                Message = "Movie retrieved successfully.",
                Data = movie
            });
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateUpdateMovieRequest request)
        {
            var movie = request.Adapt<Movie>();

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + request.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Movie", "images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }
                movie.MainImage = fileName;
            }
            var savedMovie = await _movieRepository.AddAsync(movie);
            await _movieRepository.CommitAsync();
            //Images
            if (request.MovieSubImages != null && request.MovieSubImages.Count > 0)
            {
                foreach (var image in request.MovieSubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "MovieSubImage", "images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await image.CopyToAsync(stream);
                        }
                        await _movieSubImageRepository.AddAsync(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = savedMovie.Entity.Id
                        });
                    }

                }
                await _movieRepository.CommitAsync();
            }
            return CreatedAtAction(nameof(GetAll),new ApiResponse<Movie>
            {
                IsSuccess = true,
                Message = "Movie created successfully.",
                Data = savedMovie.Entity
            });
        }

        //Update
        [HttpPut("{id}")]
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        public async Task<IActionResult> Update([FromForm] CreateUpdateMovieRequest request, int id)
        {
            var movie = await _movieRepository.GetOneAsync(c => c.Id == id, isTracked: false);
            movie = request.Adapt(movie);

            //MainImage
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Movie", "images", movie.MainImage);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                var fileName = Guid.NewGuid().ToString() + "_" + request.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Movie", "images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }
                movie.MainImage = fileName;
            }
            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync();
            //_________________________________________________________________________________
            //Images
            if (request.MovieSubImages != null && request.MovieSubImages.Count > 0)
            {
                var oldMovieSubImages = await _movieSubImageRepository.GetListAsync(m => m.MovieId == movie.Id);
                //Delete From wwwwroot
                foreach (var item in oldMovieSubImages)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "MovieSubImage", "images", item.Image);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                //Delete From DB
                _movieSubImageRepository.RemoveRange(oldMovieSubImages);
                await _movieRepository.CommitAsync();
                //_________________________________________________________________________________

                //Update MovieSubImages
                foreach (var image in request.MovieSubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        //Insert In wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "MovieSubImage", "images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        //Insert In DB
                        //Add Images In MovieSubImage Table
                        await _movieSubImageRepository.AddAsync(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = movie.Id
                        });
                    }
                }
            }
            await _movieRepository.CommitAsync();
            return Ok(new ApiResponse<Movie>
            {
                IsSuccess = true,
                Message = "Movie updated successfully.",
                Data = movie
            });
        }

        //Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Movie", "images", movie.MainImage);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            // remove MovieSubImages from wwwroot
            var oldMovieSubImages = await _movieSubImageRepository.GetListAsync(p => p.MovieId == movie.Id);
            foreach (var item in oldMovieSubImages)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "MovieSubImage", "images", item.Image);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _movieRepository.Remove(movie);
            await _movieRepository.CommitAsync();
            return NoContent();
        }
    }
}
