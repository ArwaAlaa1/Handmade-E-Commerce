using ECommerce.Core.Models;
using ECommerce.Core.Services.Contract.SendEmail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DashBoard.Areas.Identity.Pages.Account
{
    public class PinCodeConfirmationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailProvider _emailProvider;

        public PinCodeConfirmationModel(UserManager<AppUser> userManager, IEmailProvider emailProvider)
        {
            _userManager = userManager;
            _emailProvider = emailProvider;
        }


        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } 

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Range(100000, 999999, ErrorMessage = "PIN must be 6 digits.")]
            public int PIN { get; set; }

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


                if (user.ResetExpires < DateTime.Now || user.ResetExpires is null)
                {
                    TempData["Message"] = "Time Expired try to send new Pin Code.";
                    return RedirectToPage("./PinCodeConfirmation");
                }

                if (user.PasswordResetPin != Input.PIN)
                {
                    TempData["Message"] = "Invalid Pin Code.";
                    return RedirectToPage("./PinCodeConfirmation");
                }

                user.ResetExpires = null;
                user.PasswordResetPin = null;
                await _userManager.UpdateAsync(user);

                return RedirectToPage("Identity/Account/ResetPassword", new { email = Email });
            }

            return Page();
        }
    }
}
