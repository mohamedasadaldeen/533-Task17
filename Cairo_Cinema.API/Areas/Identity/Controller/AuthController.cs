using Cairo_Cinema.API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Cairo_Cinema.API.DTOs.Response;
namespace Cairo_Cinema.API.Areas.Identity.Controller
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOtp> _applicationUserOtpRepository;
        private readonly IEmailSender _emailSender;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IRepository<ApplicationUserOtp> applicationUserOtpRepository, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationUserOtpRepository = applicationUserOtpRepository;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {

            ApplicationUser user = new ApplicationUser()
            {
                Name = registerRequest.Name,
                Address = registerRequest.Address,
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Register Failed",
                    Data = result.Errors.Select(e => e.Description)
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", id = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                registerRequest.Email,
                "Cairo_Cinema Confirmation Email",
                $"<h3>Please Click <a href='{link}'>here</a> to confirm your Email</h3>"
            );

            await _userManager.AddToRoleAsync(user, CD.CUSTOMER);
            return CreatedAtAction("Login", new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Register Successfully, Please Check Your Email To Confirm Your Account",
                Data = null
            });

        }
    }
}
