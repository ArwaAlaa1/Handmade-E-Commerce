using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.Helper;
using ECommerce.DashBoard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.DashBoard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var categories =await _unitOfWork.Repository<Category>().GetAll();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        { 
           var category= await _unitOfWork.Repository<Category>().GetById(id);
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
                _unitOfWork.Repository<Category>().Add(newCategory);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetById(id);
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
                var categoryToUpdate = await _unitOfWork.Repository<Category>().GetById(id);
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

            var category = await _unitOfWork.Repository<Category>().GetById(id);
            category.IsDeleted = true;
            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
