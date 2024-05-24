using FCMS.Model.Enum;

namespace FCMS.Model.Entities
{
    public class Order : BaseEntity
    {
        public string ProductId { get; set; } = default!;
        public string PaymentId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public OrderStatus Status { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }

    }
}
