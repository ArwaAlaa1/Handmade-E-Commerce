using ECommerce.Core.Models;
using ECommerce.Core.Services.Contract;
using ECommerce.Core.Services.Contract.SendEmail;
using ECommerce.DTOs.IdentityDtos;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailProvider _emailProvider;

        public AccountController(IAuthService authService, UserManager<AppUser> userManager
            , SignInManager<AppUser> signInManager,IEmailProvider emailProvider)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailProvider = emailProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber,
            };
            
           
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);
            else
                _userManager.AddToRoleAsync(user, SD.CustomerRole).Wait();
            var token = await _authService.CreateTokenAsync(user);
            var tokene = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(
                "ConfirmEmail", 
                "Account",      
                new { userId = user.Id, token = tokene },
                protocol: HttpContext.Request.Scheme); 

           
            await _emailProvider.SendConfirmAccount(user.Email, confirmationLink);
            return Ok(new UserDto()
            {
                Token = token,
                DisplayName = user.DisplayName,
                Email = user.Email,

            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName) ??
                 await _userManager.FindByNameAsync(loginDto.EmailOrUserName);

          if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new {Message= "Please confirm your email before signing in." });
            }
            if (user == null) return Unauthorized(new
            {
                Message = "UserName or Email is InValid"
            });

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new
                {

                    Message = "Invalid Password !"
                });
         
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user),
                Image = user.Photo
            });
        }

        
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok("Email confirmed successfully");

            return BadRequest("Email confirmation failed");
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new
            {
                Message = "Logout Successfully"
            });
        }

        
        [HttpPost("send_reset_code")]
        public async Task<IActionResult> SendResetCode(SendPINDto model, [FromServices] IEmailProvider _emailProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid ModelState"
                });
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return NotFound(new
                {
                    status = 404,
                    errorMessage = "Email Not Found!"
                });
            }

            int pin = await _emailProvider.SendResetCode(model.Email);
            user.PasswordResetPin = pin;
            user.ResetExpires = DateTime.Now.AddMinutes(15);
            var expireTime = user.ResetExpires.Value.ToString("hh:mm tt");
            await _userManager.UpdateAsync(user);
            return Ok(new
            {
                status = 200,
                ExpireAt = "expired at " + expireTime,
                email = model.Email,
            });
        }
        
        [HttpPost("verify_pin/{email}")]
        public async Task<IActionResult> VerifyPin([FromBody] VerfiyPINDto model, [FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid ModelState"
                });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return NotFound(new
                {
                    status = 404,
                    errorMessage = "Email Not Found!"
                });
            }
            if (user.ResetExpires < DateTime.Now || user.ResetExpires is null)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Time Expired try to send new pin"
                });
            }
            if (user.PasswordResetPin != model.pin)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid pin"
                });
            }
            user.ResetExpires = null;
            user.PasswordResetPin = null;
            await _userManager.UpdateAsync(user);
            return Ok(new
            {
                status = 200,
                message = "PIN verified successfully",
                email = user.Email,
            });
        }

        [HttpPost("forget_password/{email}")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPassDto model, [FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid model state."
                });
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "New password and confirm new password do not match."
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Email Not Found!"
                });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
                return Ok(new
                {
                    status = 200,
                    message = "Password changed successfully"
                });
            }
            return BadRequest(new
            {
                status = 400,
                errorMessage = "Invalid model state."
            });
        }

        [Authorize]
        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return BadRequest(new
                {
                    message = "User Not Authorized !"
                });
            }

            var checkoldpass = _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!checkoldpass.Result)
            {
                return BadRequest(new
                {
                    message = "Old Password is not correct !"
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Failure in Change Password!"
                });
                
            }
            else
            {
                await _signInManager.RefreshSignInAsync(user);

                return Ok(new
                {

                    Message = "Password Change Sucessfully !"
                });

            }
        }
    
    }

}
