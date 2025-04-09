namespace ECommerce.DashBoard.ViewModels
{
    public class TraderVM
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public IFormFile Photo { get; set; }
        public string PhoneNumber { get; set; }
      
        public bool IsActive { get; set; } = true;
    }
}
