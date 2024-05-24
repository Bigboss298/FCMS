using FCMS.Model.Entities;
using FCMS.Model.Enum;

namespace FCMS.Model.DTOs
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; } = default!;
        public string PaymentId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public OrderStatus Status { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string DateCreated { get; set; }
    }

    public class UpdateOrderRequestModel
    {
        public string ProductId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
