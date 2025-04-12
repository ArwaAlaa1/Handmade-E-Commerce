using System.ComponentModel.DataAnnotations;

namespace ECommerce.DashBoard.ViewModels
{
    public class CategoryVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(80)]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

       
        public IFormFile? Photo { get; set; }
        public string? PhotoName { get; set; }

    }
}
