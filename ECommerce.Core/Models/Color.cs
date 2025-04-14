using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace ECommerce.Core.Models
{
    public class Color : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; }

        [ValidateNever]
        public ICollection<ProductColor> ProductColors { get; set; }

    }
}
