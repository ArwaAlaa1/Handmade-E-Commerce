using ECommerce.Core.Models;
using ECommerce.Core;
using ECommerce.DashBoard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.DashBoard.Controllers
{
    public class SaleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var sales = await _unitOfWork.Repository<Sale>()
                .GetAllAsync(includeProperties: "Product");
            return View(sales);
        }

        // GET: Sale/Create
        public async Task<IActionResult> Create()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            var vm = new SaleVM
            {
                Products = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(vm);
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleVM vm)
        {
            if (vm.EndDate <= vm.StartDate)
                ModelState.AddModelError("EndDate", "End Date must be after Start Date.");

            if (!ModelState.IsValid)
            {
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                vm.Products = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                });
                return View(vm);
            }

            var sale = new Sale
            {
                Percent = vm.Percent,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                ProductId = vm.ProductId
            };

            await _unitOfWork.Repository<Sale>().AddAsync(sale);
            await _unitOfWork.SaveAsync();

            return RedirectToAction("Index","Product"/*nameof(Index)*/);
        }

        // GET: Sale/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _unitOfWork.Repository<Sale>().GetByIdAsync(id);
            if (sale == null) return NotFound();

            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            var vm = new SaleVM
            {
                Id = sale.Id,
                Percent = sale.Percent,
                StartDate = sale.StartDate,
                EndDate = sale.EndDate,
                ProductId = sale.ProductId,
                Products = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(vm);
        }

        // POST: Sale/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaleVM vm)
        {
            if (vm.EndDate <= vm.StartDate)
                ModelState.AddModelError("EndDate", "End Date must be after Start Date.");

            if (!ModelState.IsValid)
            {
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                vm.Products = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                });
                return View(vm);
            }

            var sale = await _unitOfWork.Repository<Sale>().GetByIdAsync(vm.Id);
            if (sale == null) return NotFound();

            sale.Percent = vm.Percent;
            sale.StartDate = vm.StartDate;
            sale.EndDate = vm.EndDate;
            sale.ProductId = vm.ProductId;

            _unitOfWork.Repository<Sale>().Update(sale);
            await _unitOfWork.SaveAsync();

            return RedirectToAction("Index","Product"/*nameof(Index)*/);
        }

        // GET: Sale/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _unitOfWork.Repository<Sale>().GetByIdWithIncludeAsync(id, "Product");
            if (sale == null) return NotFound();

            return View(sale);
        }


        // POST: Sale/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _unitOfWork.Repository<Sale>()
                .GetByIdWithIncludeAsync(id, "Product");

            if (sale == null) return NotFound();

            // Optionally remove the sale from product manually (if tracked)
            if (sale.Product != null)
            {
                sale.Product.Sales.Remove(sale); // Only if needed
            }

            _unitOfWork.Repository<Sale>().Delete(sale);
            await _unitOfWork.SaveAsync();

            return RedirectToAction("Index","Product"/*nameof(Index)*/);
        }


        /**/

        //public async Task<IActionResult> Manage(int productId)
        //{

        //    var existingSale = await _unitOfWork.Repository<Sale>()
        //        .GetByIdAsync(productId);


        //    var products = await _unitOfWork.Repository<Product>().GetAllAsync();

        //    var vm = new SaleVM
        //    {
        //        ProductId = productId,
        //        Products = products.Select(p => new SelectListItem
        //        {
        //            Value = p.Id.ToString(),
        //            Text = p.Name
        //        }),


        //        Id = existingSale?.Id ?? 0,
        //        Percent = existingSale?.Percent ?? 0,
        //        StartDate = existingSale?.StartDate ?? DateTime.Today,
        //        EndDate = existingSale?.EndDate ?? DateTime.Today.AddDays(7)
        //    };

        //    return View(vm); 
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Manage(SaleVM vm)
        //{
        //    if (vm.EndDate <= vm.StartDate)
        //        ModelState.AddModelError("EndDate", "End Date must be after Start Date.");

        //    if (!ModelState.IsValid)
        //    {
        //        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        //        vm.Products = products.Select(p => new SelectListItem
        //        {
        //            Value = p.Id.ToString(),
        //            Text = p.Name
        //        });

        //        return View(vm);
        //    }


        //    Sale sale;
        //    if (vm.Id == 0)
        //    {

        //        sale = new Sale
        //        {
        //            ProductId = vm.ProductId,
        //            Percent = vm.Percent,
        //            StartDate = vm.StartDate,
        //            EndDate = vm.EndDate
        //        };
        //        await _unitOfWork.Repository<Sale>().AddAsync(sale);
        //    }
        //    else
        //    {

        //        sale = await _unitOfWork.Repository<Sale>().GetByIdAsync(vm.Id);
        //        if (sale == null) return NotFound();

        //        sale.Percent = vm.Percent;
        //        sale.StartDate = vm.StartDate;
        //        sale.EndDate = vm.EndDate;
        //        sale.ProductId = vm.ProductId;

        //        _unitOfWork.Repository<Sale>().Update(sale);
        //    }

        //    await _unitOfWork.SaveAsync();

        //    return RedirectToAction("Index", "Product");
        //}





    }

}
