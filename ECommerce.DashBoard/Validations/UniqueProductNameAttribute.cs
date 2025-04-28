using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.ViewModels;

namespace ECommerce.DashBoard.Validations
{
    public class UniqueProductNameAttribute :ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            return IsValidAsync(value, validationContext).GetAwaiter().GetResult();
        }

        private async Task<ValidationResult?> IsValidAsync(object? value, ValidationContext validationContext)
        {
            var productVM = validationContext.ObjectInstance as ProductVM;
            if (productVM == null)
            {
                return new ValidationResult("Invalid product instance.");
            }

            var productName = value as string;
            if (string.IsNullOrEmpty(productName))
            {
                return ValidationResult.Success; // Let [Required] handle empty values if needed
            }

            var unitOfWork = validationContext.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            if (unitOfWork == null)
            {
                return new ValidationResult("Unable to access unit of work.");
            }
            var productRepository = unitOfWork.Repository<Product>();
            var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            if (httpContextAccessor == null || httpContextAccessor.HttpContext == null)
            {
                return new ValidationResult("Unable to access user context.");
            }

            var sellerId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sellerId))
            {
                return new ValidationResult("Unable to identify the seller.");
            }
            // Use FirstOrDefault on the IEnumerable<Product>
            var existingProduct = await productRepository.GetFirstOrDefaultAsync(
                p => p.Name == productName && p.SellerId == sellerId && p.Id != productVM.Id);


            if (existingProduct != null)
            {
                return new ValidationResult($"A product with the name '{productName}' already exists for this trader.");
            }

            return ValidationResult.Success;
        }
    }
}
