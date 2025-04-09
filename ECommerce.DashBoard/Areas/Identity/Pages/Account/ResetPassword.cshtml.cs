using ECommerce.Core.Models;
using ECommerce.Core.Services.Contract.SendEmail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DashBoard.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Pin { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "New Password is required")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
                ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character.")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Confirm Password is required")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
            [DataType(DataType.Password)]
            public string ConfirmNewPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    TempData["Message"] = "Email Not Found!";

                    return RedirectToPage("./ForgotPassword");
                }

                if (user.PasswordResetPin != Pin)
                {
                    TempData["Message"] = "Invalid Pin Code.";
                    return RedirectToPage("./PinCodeConfirmation", new { email = Email });
                }

                if (Input.NewPassword != Input.ConfirmNewPassword)
                {
                    TempData["Message"] = "New password and confirm new password do not match.";
                    return RedirectToPage("./ResetPassword", new { email = Email , pin = Pin});
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);

                if (result.Succeeded)
                {
                    user.PasswordResetPin = null;
                    await _userManager.UpdateAsync(user);
                    TempData["Message"] = "Password changed successfully";
                    return RedirectToPage("Identity/Account/Login");
                }
                return RedirectToPage("./ForgotPassword");
            }
            return Page();
        }
    }
}