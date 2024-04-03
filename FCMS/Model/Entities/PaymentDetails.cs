using FCMS.Model.Enum;

namespace FCMS.Model.Entities
{
    public class PaymentDetails : BaseEntity
    {
        public BankType Type { get; set; } = default!;
        public string? Recipient_Code { get; set; } 
        public string BankCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Currency { get; set; }
        public string FarmerId { get; set; } = default!;
        public Farmer Farmer { get; set; } = default!;
    }
}
