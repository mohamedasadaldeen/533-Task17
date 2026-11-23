using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.API.Areas.Identity.Controller
{
    [Area(CD.IDENTITY_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetDetails()
        {
            var user = await _userManager.GetUserAsync(User);
            var userResponse = user.Adapt<ApplicationUserResponse>();
            return Ok(new ApiResponse<ApplicationUserResponse>
            {
                IsSuccess = true,
                Message = "User details retrieved successfully.",
                Data = userResponse
            });
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ApplicationUserRequest applicationUserRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            user.UserName = applicationUserRequest.UserName;
            user.Name = applicationUserRequest.Name;
            user.PhoneNumber = applicationUserRequest.PhoneNumber;
            user.Address = applicationUserRequest.Address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Failed to update profile.",
                    Data = result.Errors.Select(e => e.Description)
                });
            }
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Profile updated successfully.",
                Data = null
            });

        }
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(ChangePasswordRequest changePasswordRequest)
        {

            var user = await _userManager.GetUserAsync(User);
            //كود يفحص اذا حقل الباسورد فارغ 
            if (string.IsNullOrWhiteSpace(changePasswordRequest.CurrentPassword) ||
                 string.IsNullOrWhiteSpace(changePasswordRequest.NewPassword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Current password and new password are required.",
                    Data = null
                });
            }
            //
            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Failed to update password.",
                    Data = result.Errors.Select(e => e.Description)
                });
            }
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Password updated successfully.",
                Data = null
            });
        }
    }
}
