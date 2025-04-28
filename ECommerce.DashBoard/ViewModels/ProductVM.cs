using ECommerce.Core.Models;
using ECommerce.DashBoard.Validations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ECommerce.DashBoard.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100)]
        [UniqueProductName(ErrorMessage = "A product with this name already exists for the trader.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Cost { get; set; }
        public decimal? AdminProfitPercentage { get; set; }
        public decimal SellingPrice { get; set; }
        public string? CategoryName { get; set; }
        public string? AdditionalDetails { get; set; }


        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

        //public List<int> SelectedColorIds { get; set; } = new();
        //public List<int> SelectedSizeIds { get; set; } = new();

        // New: Colors and Sizes to be added dynamically
        public List<ColorVM> Colors { get; set; } = new();
        public List<SizeVM> Sizes { get; set; } = new();


        //public IEnumerable<SelectListItem> AvailableColors { get; set; } = new List<SelectListItem>();
        //public IEnumerable<SelectListItem> AvailableSizes { get; set; } = new List<SelectListItem>();


        //  Handle new uploaded photos
        [ValidateNever]
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();

        // Display existing photos in edit view
        public List<string> ExistingPhotoLinks { get; set; } = new List<string>();
        public List<ProductPhotoVM> ExistingPhotoLinksWithIds { get; set; } = new();

        public bool IsOnSale { get; set; }
        public int? SaleId { get; set; }
        public int? SalePercent { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public decimal? DiscountedPrice { get; set; }

    }

}

