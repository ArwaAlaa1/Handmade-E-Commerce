using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.Helper;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.DashBoard.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public CategoryController(IUnitOfWork unitOfWork ,UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //var user =await _userManager.GetUserAsync(User);
            var categories =await _unitOfWork.Repository<Category>().GetAllAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        { 
           var category= await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM category)
        {
            if (ModelState.IsValid)
            {
                var imageName="";
                if (category.Photo != null)
                {
                     imageName=HandlerPhotos.UploadPhoto(category.Photo, "Categories");
                }
                var newCategory = new Category
                {
                    Name = category.Name,
                    Description = category.Description,
                    Photo = imageName
                };
                await _unitOfWork.Repository<Category>().AddAsync(newCategory);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null) return NotFound();
            var categoryVM = new CategoryVM
            {
                
                Name = category.Name,
                Description = category.Description,
               PhotoName = category.Photo
            };
            return View(categoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,CategoryVM categoryVM)
        {

            if (ModelState.IsValid)
            {
                var categoryToUpdate = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
                if (categoryToUpdate == null) return NotFound();
                if (categoryVM.Photo != null)
                {
                    if (categoryToUpdate.Photo != null)
                    {
                        HandlerPhotos.DeletePhoto("Categories", categoryToUpdate.Photo);
                    }
                    var imageName = HandlerPhotos.UploadPhoto(categoryVM.Photo, "Categories");
                    categoryToUpdate.Name = categoryVM.Name;
                    categoryToUpdate.Description = categoryVM.Description;
                    categoryToUpdate.Photo = imageName;
                }
                else
                {
                    categoryToUpdate.Name = categoryVM.Name;
                    categoryToUpdate.Description = categoryVM.Description;
                }
                 _unitOfWork.Repository<Category>().Update(categoryToUpdate);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryVM);
        }

        public async Task<IActionResult> Delete(int id)
        {

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        
            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
