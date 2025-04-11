using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DashBoard.ViewModels
{
    public class SaleVM
    {
        public int Id { get; set; }


        [Required, Range(1, 100)]
        public int Percent { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Products { get; set; }
    }

}
