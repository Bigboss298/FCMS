namespace FCMS.Gateway
{
    public class PaymentDto
    {

    }
    public class CreatePaymentRequestModel
    {
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string? reason { get; set; } 
        public string PhoneNumber { get; set; }
        public string CustomerId { get; set; }
        public string productId { get; set; }
        public int quantity { get; set; }
        public string recipient { get; set; }
    }
    public class PaymentInitalizationResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public SubData data { get; set; }
    }

    public class SubData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
