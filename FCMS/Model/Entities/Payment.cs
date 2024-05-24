using FCMS.Model.Enum;

namespace FCMS.Model.Entities
{
    public class Payment : BaseEntity
    {
       
        public string? ReferenceNumber { get; set; }
        public string? AuthorizationUri { get; set; }
        public decimal Amount { get; set; }
        public string? Method { get; set; } 
        public string? TransactionId { get; set; }
        public string? OrderId { get; set; }
        public bool IsPaid { get; set; }
        public PaymentStatus Status { get; set; }
        public string CustomerId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public Customer Customer { get; set; } = default!;
        public Product Product { get; set; } = default!;

    }
}
