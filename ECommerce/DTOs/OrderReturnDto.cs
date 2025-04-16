namespace ECommerce.DTOs
{
    public class OrderReturnDto
    {
        public int Id { get; set; }

        public string OrderDate { get; set; }
        public string Status { get; set; }
        public int ItemsCount { get; set; }
       
        public decimal Total { get; set; }
    }
}
