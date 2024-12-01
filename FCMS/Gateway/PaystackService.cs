using FCMS.Implementations.Repository;
using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Model.Enum;
using FCMS.Model.Exceptions;
using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using Paystack.Net.SDK;
using Paystack.Net.SDK.Models;
using Paystack.Net.SDK.Models.Transfers.TransferDetails;
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
        private readonly IPaymentDetails _paymentDetails;
        private readonly IFarmerRepository _farmerRepository;

        public PaystackService(ICustomerRepository customerRepository, IProductRepository productRepository, IConfiguration configuration, IUnitOfWork unitOfWork, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IProductOrderRepository productOrderRepository, IPaymentDetails paymentDetails, IFarmerRepository farmerRepository)
        {
            _customerRepoitory = customerRepository;
            _productRepository = productRepository;
            _secretKey = configuration["Paystack:ApiKey"];
            client = new HttpClient();
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _productOrderRepository = productOrderRepository;
            _paymentDetails = paymentDetails;
            _farmerRepository = farmerRepository;
        }
        public async Task<string> GetTransactionRecieptAsync(string transactionReference)
        {
            if (transactionReference is null)
            {
                throw new BadRequestException("Reference Number cant be null");
            }
            //var payment = await _paymentRepository.Get<Payment>(x => x.ReferenceNumber == transactionReference);
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

        public async Task<string> PayFarmer(CreatePaymentRequestModel model)
        {
            if (model is null)
            {
                throw new BadRequestException("Model cant be null");
            }
            var product = await _productRepository.Get(x => x.Id == model.productId);
            if (product is null)
            {
                throw new NotFoundException("Product not found");
            }
            var customer = await _customerRepoitory.Get(x => x.Id == model.CustomerId);
            if (customer is null)
            {
                throw new NotFoundException("customer not found");
            }
            var paymentDetails = await _paymentDetails.Get<PaymentDetails>(x => x.FarmerId == product.FarmerId);
            if (paymentDetails is null)
            {
                throw new NotFoundException("farmer paymentDetails not found");
            }
            var farmer = await _farmerRepository.Get(x => x.Id == product.FarmerId);
            var order = await _orderRepository.Get<Order>(x => x.Id == model.OrderId);
            
            var payment = await _paymentRepository.Get<Payment>(x => x.OrderId == order.Id);

            if (farmer is null || order is null || payment is null)
            {
                throw new NotFoundException("Something went wrong");
            }


            string url = "https://api.paystack.co/transfer";
            string authorization = $"Bearer {_secretKey}";
            string contentType = "application/json";

            string jsonData = $@"
                {{
                  ""source"": ""balance"",
                  ""reason"": ""payment for Product {model.productId}"",
                  ""amount"": {order.Price},
                  ""recipient"": ""{paymentDetails.Recipient_Code}""
                }}";

            var content = new StringContent(jsonData, Encoding.UTF8, contentType);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", authorization);

            HttpResponseMessage response = await client.PostAsync(url, content);


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                payment.Status = PaymentStatus.success;
                
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                return responseBody;
            }
            else
            {
                throw new Exception($"Payment initiation failed. Response: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<string> Payment(RequestPaymentModel model)
        {
            if (model is null) throw new BadRequestException("Model can't be null!!");

            var getCustomer = await _customerRepoitory.Get(x => x.UserId == model.UserId);
            var getProduct = await _productRepository.Get(x => x.Id == model.ProductId);

            if (getProduct.Quantity < model.Quantity) throw new BadRequestException($"We have just only {getProduct.Quantity} in store");

            Order order = new()
            {
                ProductId = getProduct.Id,
                Product = getProduct,
                Quantity = model.Quantity,
                Price = model.Amount,
                CustomerId = getCustomer.Id,
                Customer = getCustomer,
            };

            Payment payment = new()
            {
                CustomerId = getCustomer.Id,
                ProductId = getProduct.Id,
                Customer = getCustomer,
                OrderId = order.Id,
                Status = PaymentStatus.pending,
                Amount = model.Amount,
            };

            var url = "https://api.paystack.co/transaction/initialize";
            var baseUrl = "https://fcms-web-app.vercel.app";
            var callBackUrl = $"{baseUrl}/myorders";
            var secretKey = _secretKey;
            var email = model.Email;
            var amount = model.Amount * 100;

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

                var data = new
                {
                    email = email,
                    amount = amount,
                    callback_url = callBackUrl,
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Status Code: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                    string authorizationUrl = responseObject.data.authorization_url;

                payment.AuthorizationUri = authorizationUrl;
                payment.ReferenceNumber = responseObject.data.reference;
                order.PaymentId = payment.Id;
                order.Status = OrderStatus.Ordered;  
                getProduct.Quantity -= model.Quantity;

                 _orderRepository.Insert<Order>(order);
                _paymentRepository.Insert<Payment>(payment);

                await _unitOfWork.SaveChangesAsync();

                    return authorizationUrl;
                }
                else
                {
                    throw new Exception("Payment request failed.");
                }
        }

    }
}