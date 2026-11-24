using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.API.Areas.Admin.Controller
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN},{CD.EMPLOYEE}")]
    public class ActorController : ControllerBase
    {
        private readonly IRepository<Actor> _actorRepository;
        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actors = await _actorRepository.GetListAsync();
            return Ok(new ApiResponse<IEnumerable<Actor>>
            {
                IsSuccess = true,
                Message = "Actors retrieved successfully.",
                Data = actors
            });
        }

        //GetOne
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id);

            if (actor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Actor not found.",
                    Data = null
                });
            }
            return Ok(new ApiResponse<Actor>
            {
                IsSuccess = true,
                Message = "Actor retrieved successfully.",
                Data = actor
            });
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUpdateActorRequest createUpdateActorRequest)
        {
            var actor = createUpdateActorRequest.Adapt<Actor>();

            if (createUpdateActorRequest.ImageFile != null && createUpdateActorRequest.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + createUpdateActorRequest.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Actor", "images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createUpdateActorRequest.ImageFile.CopyTo(stream);
                }
                actor.Image = fileName;
            }

            await _actorRepository.AddAsync(actor);
            await _actorRepository.CommitAsync();

            return CreatedAtAction(nameof(GetAll), new ApiResponse<Actor>
            {
                IsSuccess = true,
                Message = "Actor created successfully.",
                Data = actor
            });
        }

        //Update
        [HttpPut("{id}")]
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateUpdateActorRequest createUpdateActorRequest)
        {

            var actor = await _actorRepository.GetOneAsync(c => c.Id == id, isTracked: false);
            if (actor == null) return NotFound();
            actor = createUpdateActorRequest.Adapt(actor);

            if (createUpdateActorRequest.ImageFile != null && createUpdateActorRequest.ImageFile.Length > 0)
            {
                //Delete old Image
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Actor", "images", actor.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                //Create New Image
                var fileName = Guid.NewGuid().ToString() + "_" + createUpdateActorRequest.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Actor", "images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await createUpdateActorRequest.ImageFile.CopyToAsync(stream);
                }
                actor.Image = fileName;
            }

            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync();
            return Ok(new ApiResponse<Actor>
            {
                IsSuccess = true,
                Message = "Actor updated successfully.",
                Data = actor
            });
        }

        //Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id);
            if (actor == null) return NotFound();
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Actor", "images", actor.Image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            _actorRepository.Remove(actor);
            await _actorRepository.CommitAsync();
            return NoContent();
        }
    }
}
