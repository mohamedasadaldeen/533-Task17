using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.API.Areas.Admin.Controller
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN},{CD.EMPLOYEE}")]

    public class CinemaController : ControllerBase
    {
        private readonly IRepository<Cinema> _cinemaRepository;

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cinemas = await _cinemaRepository.GetListAsync();

            return Ok(new ApiResponse<IEnumerable<Cinema>>
            {
                IsSuccess = true,
                Message = "Cinemas retrieved successfully",
                Data = cinemas
            });
        }

        //GetOne
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);

            if (cinema == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Cinema Not Found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Cinema>
            {
                IsSuccess = true,
                Message = "Cinema Retrieved Successfully",
                Data = cinema
            });
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUpdateCinemaRequest request)
        {
            var cinema = request.Adapt<Cinema>();
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + request.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Cinema", "Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }
                cinema.Image = fileName;
            }

            await _cinemaRepository.AddAsync(cinema);
            await _cinemaRepository.CommitAsync();
            return CreatedAtAction(nameof(GetAll), new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Category Created Successfully",
                Data = null
            });
        }

        //Update
        [HttpPut("{id}")]
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        public async Task<IActionResult> Update([FromForm] CreateUpdateCinemaRequest request, int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id, isTracked: false);
            cinema = request.Adapt(cinema);

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Cinema", "Images", cinema.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                var fileName = Guid.NewGuid().ToString() + "_" + request.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Cinema", "Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }
                cinema.Image = fileName;
            }

            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Category Updated Successfully",
                Data = null
            });
        }

        //Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Cinema", "Images", cinema.Image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            _cinemaRepository.Remove(cinema);
            await _cinemaRepository.CommitAsync();
            return NoContent();

        }

    }
}
