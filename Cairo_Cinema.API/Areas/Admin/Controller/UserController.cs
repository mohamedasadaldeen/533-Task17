using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Cairo_Cinema.API.Areas.Admin.Controller
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{CD.SUPER_ADMIN}, {CD.ADMIN}")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = _userManager.Users.AsEnumerable();
            return Ok(new ApiResponse<IEnumerable<ApplicationUser>>
            {
                IsSuccess = true,
                Message = "Movies retrieved successfully.",
                Data = users
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            request.Roles = _roleManager.Roles.Select(r => r.Name!).ToList();
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                result.Errors.Select(e => e.Description);
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid Data",
                    Data = result.Errors.Select(e => e.Description)
                });
            }

            if (!string.IsNullOrEmpty(request.Role))
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            return CreatedAtAction("", new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "User Created Successfully",
                Data = null
            });
        }

        //LockUnLock
        [HttpPut("{id}")]
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound(new ApiResponse<object>
            {
                IsSuccess = false,
                Message = "Invalid User",
                Data = null
            });

            //Super Admin UnLock Always
            var isSuperAdmin = await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN);
            if (isSuperAdmin) 
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "You Can't Lock Super Admin !",
                    Data = null
                });
         
            }

            //UnLock
            if (user.LockoutEnd == null || DateTime.UtcNow > user.LockoutEnd)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, null);

            }

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Lock/UnLock Successfully",
                Data = null
            });
        }

        //Delete
        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = CD.SUPER_ADMIN)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            // منع حذف نفسك
            if (user.Id == _userManager.GetUserId(User))
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "You can't delete your own account.",
                    Data = null
                });
            }

            // منع حذف السوبر أدمن
            if (await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN))
            {
                
                 return BadRequest(new ApiResponse<object>
                 {
                     IsSuccess = false,
                     Message = "You can't delete a Super Admin.",
                     Data = null
                 });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid Data",
                    Data = result.Errors.Select(e => e.Description)
                });
            }
            return NoContent();

        }
    }
}
