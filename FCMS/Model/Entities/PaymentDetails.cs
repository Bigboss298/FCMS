namespace FCMS.Model.Entities
{
    public class PaymentDetails : BaseEntity
    {
        public string? Recipient_Code { get; set; } 
        public string BankCode { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string Currency { get; set; } = "NGN";
        public string FarmerId { get; set; } = default!;
        public Farmer Farmer { get; set; } = default!;
    }
}
