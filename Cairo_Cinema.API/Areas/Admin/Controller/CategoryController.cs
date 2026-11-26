using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.API.Areas.Admin.Controller
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN},{CD.EMPLOYEE}")]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetListAsync();

            return Ok(new ApiResponse<IEnumerable<Category>>
            {
                IsSuccess = true,
                Message = "Categories Retrieved Successfully",
                Data = categories
            });
        }

        //GetOne
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Category Not Found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Category>
            {
                IsSuccess = true,
                Message = "Category Retrieved Successfully",
                Data = category
            });
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateCategoryRequest CreateUpdateCategoryRequest)
        {
            /*var category = new Category();
            category.Name = CreateUpdateCategoryRequest.Name;
            category.Description = CreateUpdateCategoryRequest.Description;
            category.Status = CreateUpdateCategoryRequest.Status;*/
            var category = CreateUpdateCategoryRequest.Adapt<Category>();

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.CommitAsync();
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
        public async Task<IActionResult> Update(int id, CreateUpdateCategoryRequest CreateUpdateCategoryRequest)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            category = CreateUpdateCategoryRequest.Adapt(category);

            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Category Not Found",
                    Data = category
                });
            }

            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Category Updated Successfully",
                Data = null
            });
        }

        //Delete
        [Authorize(Roles = $"{CD.SUPER_ADMIN},{CD.ADMIN}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            _categoryRepository.Remove(category);
            await _categoryRepository.CommitAsync();
            return NoContent();
        }
    }
}
