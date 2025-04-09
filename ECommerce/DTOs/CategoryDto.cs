using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
