using ECommerce.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DashBoard.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Cost { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        //  Handle new uploaded photos
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();

        // Display existing photos in edit view
        public List<string> ExistingPhotoLinks { get; set; } = new List<string>();
        public List<ProductPhotoVM> ExistingPhotoLinksWithIds { get; set; } = new();

    }

}

