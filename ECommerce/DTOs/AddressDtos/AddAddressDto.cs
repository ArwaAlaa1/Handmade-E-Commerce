using ECommerce.Core.Models;
using System.Text.Json.Serialization;

namespace ECommerce.DTOs.AddressDtos
{
    public class AddAddressDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Region { get; set; }
        public string City { get; set; }

        public string Country { get; set; }
        public string? AddressDetails { get; set; }
    }
}
