namespace ECommerce.DTOs.OrderDtos
{
    public class AddressReturnDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string? AddressDetails { get; set; }
    }
}
