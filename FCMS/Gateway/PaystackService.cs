using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using System.Text;

namespace FCMS.Gateway
{
    public class PaystackService : IPaystackService
    {
        private static HttpClient client;
        private readonly string _secretKey;
        private readonly ICustomerRepository _customerRepoitory;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProductOrderRepository _productOrderRepository;

        public PaystackService(ICustomerRepository customerRepository, IProductRepository productRepository, IConfiguration configuration, IUnitOfWork unitOfWork, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IProductOrderRepository productOrderRepository)
        {
            _customerRepoitory = customerRepository;
            _productRepository = productRepository;
            _secretKey = configuration["Paystack:ApiKey"];
            client = new HttpClient();
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _productOrderRepository = productOrderRepository;

        }
        public async Task<string> GetTransactionRecieptAsync(string transactionReference)
        {
            if (transactionReference is null)
            {
                throw new BadRequestException("Reference Number cant be null");
            }
            var transaction = await _paymentRepository.Get<Payment>(x => x.ReferenceNumber == transactionReference);
            if (transaction is null)
            {
                throw new BadRequestException("No transaction with such transaction number");
            }
            string url = $"https://api.paystack.co/transaction/verify/{transactionReference}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_secretKey}");
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return responseContent;
            }
            else
            {
                throw new Exception($"Transaction verification failed. Response: {responseContent}");
            }
        }

        public async Task<string> InitiatePayment(CreatePaymentRequestModel model)
        {
            var product = await _productRepository.Get(x => x.Id == model.productId);
            var customer = await _customerRepoitory.Get(x => x.Id == model.CustomerId);

            if (product is null || customer is null)
            {
                throw new NotFoundException("No product or customer found");
            }

            string url = "https://api.paystack.co/transfer";
            string authorization = $"{_secretKey}";
            string contentType = "application/json";
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                source = "balance",
                reason = "Calm down",
                amount = model.Amount * 100,
                recipient = product.Farmer.PaymentDetails.Recipient_Code,
            });

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(contentType));

                var content = new StringContent(data, Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    _ = product.Quantity - model.quantity;
                    Order order = new()
                    {
                        ProductId = product.Id,
                        Quantity = model.quantity,
                        Price = model.Amount,
                        CustomerId = customer.Id,
                    };
                    Payment payment = new()
                    {
                        ReferenceNumber = $"FCMS/{Guid.NewGuid().ToString()[..10]}",
                        Amount = model.Amount,
                        TransactionId = Guid.NewGuid().ToString()[..9],
                        IsPaid = true,
                        Status = Model.Enum.PaymentStatus.success,
                        CustomerId = customer.Id,
                        ProductId = product.Id,
                    };

                    ProductOrder porder = new()
                    {
                        ProductId = product.Id,
                        OrderId = order.Id,
                    };

                    _orderRepository.Insert<Order>(order);
                    _paymentRepository.Insert<Payment>(payment);
                    _productOrderRepository.Insert<ProductOrder>(porder);
                    _productRepository.Update<Product>(product);
                    await _unitOfWork.SaveChangesAsync();

                    return responseBody;
                }
                else
                {
                    throw new Exception($"Payment initiation failed. Response: {await response.Content.ReadAsStringAsync()}");
                }
            }
        }

    }
}
