using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Cairo_Cinema.API.Areas.Identity.Controller
{
    [Area(CD.IDENTITY_AREA)]
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

        //Register
        [HttpPost("Register")]
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

        //Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = null
                });
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);
            if (!result.Succeeded)
            {
                List<string> errors = new List<string>();
                if (result.IsLockedOut)
                {
                    errors.Add("You are Locked Please again later");
                }
                else if (result.IsNotAllowed)
                {
                    errors.Add("Please Confirm your Email First");
                }
                else
                {
                    errors.Add("Invalid UserName Or Email Or Password");
                }
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Login Failed",
                    Data = errors
                });
            }

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Login Successfully",
                Data = null
            });
        }

        //ConfirmEmail
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid Confirmation Email",
                    Data = null
                });
            }
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Email Confirmed Successfully",
                Data = null
            });
        }

        //ResendEmailConfirmation
        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest resendEmailConfirmationRequest)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationRequest.UserNameOrEmail) ??
            await _userManager.FindByNameAsync(resendEmailConfirmationRequest.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid UserName Or Email",
                    Data = null
                });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Email Already Confirmed",
                    Data = null
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", id = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Cairo_Cinema Confirmation Email",
                $"<h3>Please Click <a href='{link}'>here</a> to confirm your Email</h3>"
            );
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Email Sent Successfully",
                Data = null
            });
        }

        //ForgetPassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest rorgetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(rorgetPasswordRequest.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(rorgetPasswordRequest.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid UserName Or Email",
                    Data = null
                });
            }

            var otp = new Random().Next(10000, 99999).ToString();
            var applicationUserOtp = new ApplicationUserOtp(otp, user.Id);
            await _applicationUserOtpRepository.AddAsync(applicationUserOtp);
            await _applicationUserOtpRepository.CommitAsync();
            await _emailSender.SendEmailAsync(
                user.Email,
                "Cairo_Cinema Reset password",
                $"<h3>Use This {otp} to Reset Your Password</h3>"
            );
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "OTP Sent Successfully",
                Data = user.Id
            });
        }

        //VerifyOtp
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest verifyOtpRequest)
        {
            var user = await _userManager.FindByIdAsync(verifyOtpRequest.UserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid User",
                    Data = null
                });
            }
            var applicationUserOtps = await _applicationUserOtpRepository.GetListAsync(a =>
                a.ApplicationUserId == user.Id &&
                a.IsValid &&
                a.ValidTo > DateTime.UtcNow
            );
            var applicationUserOtp = applicationUserOtps.OrderBy(a => a.ApplicationUserId).LastOrDefault();
            if (applicationUserOtp == null || applicationUserOtp.OTP != verifyOtpRequest.OTP)
            {

                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid OTP / Expired",
                    Data = null
                });

            }
            applicationUserOtp.IsValid = false;
            await _applicationUserOtpRepository.CommitAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "OTP Verified Successfully",
                Data = new { UserId = user.Id, Token = token }
            });
        }

        //ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest verifyOtpRequest)
        {

            var user = await _userManager.FindByIdAsync(verifyOtpRequest.UserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid User",
                    Data = null
                });
            }
            var result = await _userManager.ResetPasswordAsync(user, verifyOtpRequest.Token, verifyOtpRequest.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Reset Password Failed",
                    Data = result.Errors.Select(e => e.Description)
                });
            }
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Reset Password Successfully",
                Data = null
            });

        }

        //Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();

        }
    }
}
