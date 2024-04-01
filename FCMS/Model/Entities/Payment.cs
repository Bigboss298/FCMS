using FCMS.Model.Enum;

namespace FCMS.Model.Entities
{
    public class Payment : BaseEntity
    {
       
        public string ReferenceNumber { get; set; } = default!;
        public string? AuthorizationUri { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } 
        public string TransactionId { get; set; } = default!;
        public bool IsPaid { get; set; }
        public PaymentStatus Status { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public Customer Customer { get; set; } = default!;
        public Product Product { get; set; } = default!;

    }
}
