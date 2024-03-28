using FCMS.Model.Entities;
using FCMS.Model.Enum;

namespace FCMS.Model.DTOs
{
    public class PaymentDto
    {
        public string ReferenceNumber { get; set; } = default!;
        public string AuthorizationUri { get; set; } = default!;
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string TransactionId { get; set; } = default!;
        public bool IsPaid { get; set; } = false;
        public PaymentStatus Status { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public Customer Customer { get; set; } = default!;
        public ProductDto Product { get; set; } = default!;
    }
    
    public class UpdatePaymentRequestModel
    {
        public bool IsPaid { get; set; }
    }
}
