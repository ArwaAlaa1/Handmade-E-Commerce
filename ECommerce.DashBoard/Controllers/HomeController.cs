using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.Data;
using ECommerce.DashBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.DashBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _untuOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ECommerceDbContext _context;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ECommerceDbContext context)
        {
            _logger = logger;
            _untuOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user01 = await _context.Users.FindAsync(userId);

            var role01 = await (from role in _context.Roles
                              join userRole in _context.UserRoles
                              on role.Id equals userRole.RoleId
                              join user in _context.Users
                              on userRole.UserId equals user.Id
                              where user.Id == userId
                              select role)
                  .FirstOrDefaultAsync();


            ViewBag.userData = user01;
            ViewBag.roleData = role01;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
