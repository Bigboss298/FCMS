namespace FCMS.Gateway
{
    public class PaymentDto
    {

    }
    public class RequestPaymentModel
    {
        public string Email { get; set; } = default!;
        public int Amount { get; set; }
        public string ProductId { get; set; } = default!;
        public string UserId { get; set; } = default!;  
        public int Quantity { get; set; }   
    }
    public class CreatePaymentRequestModel
    {
        public string CustomerId { get; set; }
        public string productId { get; set; }
        public string OrderId { get; set; }
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
