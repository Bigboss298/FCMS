namespace FCMS.Gateway
{
    public class PaymentDto
    {

    }
    public class CreatePaymentRequestModel
    {
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerId { get; set; }
        public string productId { get; set; }
        public int quantity { get; set; }
    }
}
