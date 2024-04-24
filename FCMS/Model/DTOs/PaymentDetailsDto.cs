using FCMS.Model.Entities;

namespace FCMS.Model.DTOs
{
    public class PaymentDetailsDto
    {
        public string BankCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string? AccountType { get; set; } 
        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;
    }

    public class RecipientData
    {
        public string recipient_code { get; set; } = default!;
    }
}
