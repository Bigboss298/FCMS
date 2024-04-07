using FCMS.Implementations.Repository;
using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
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

        public PaystackService(ICustomerRepository customerRepository, IProductRepository productRepository, IConfiguration configuration, IUnitOfWork unitOfWork, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IProductOrderRepository productOrderRepository, IPaymentDetails paymentDetails)
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
            if(model is null)
            {
                throw new BadRequestException("Model cant be null");
            }
            var product = await _productRepository.Get(x => x.Id == model.productId);
            var customer = await _customerRepoitory.Get(x => x.Id == model.CustomerId);
            var paymentDetails = await _paymentDetails.Get<PaymentDetails>(x => x.FarmerId == product.FarmerId);

            if (product is null || customer is null)
            {
                throw new NotFoundException("No product or customer found");
            }

            string url = "https://api.paystack.co/transfer";
            string authorization = $"Bearer {_secretKey}";
            string contentType = "application/json";

            string jsonData = $@"
        {{
          ""source"": ""balance"",
          ""reason"": ""{model.reason}"",
          ""amount"": {model.Amount},
          ""recipient"": ""{model.recipient}""
        }}";

            var content = new StringContent(jsonData, Encoding.UTF8, contentType);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", authorization);

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


        //public async Task<PaymentInitalizationResponseModel> InitializeTransaction(TransactionInitializationRequestModel requestObj)
        //{

        //    var client = HttpConnection.CreateClient(this._secretKey);

        //    var bodyKeyValues = new List<KeyValuePair<string, string>>();


        //    foreach (var property in requestObj.GetType().GetProperties())
        //    {
        //        if (property.GetValue(requestObj) != null)
        //        {
        //            bodyKeyValues.Add(new KeyValuePair<string, string>(property.Name, property.GetValue(requestObj).ToString()));
        //        }
        //    }

        //    var formContent = new FormUrlEncodedContent(bodyKeyValues);

        //    var response = await client.PostAsync("transaction/initialize", formContent);

        //    var json = await response.Content.ReadAsStringAsync();

        //    return JsonConvert.DeserializeObject<PaymentInitalizationResponseModel>(json);
        //}

        //public async Task<TransactionResponseModel> VerifyTransaction(string reference)
        //{
        //    var client = HttpConnection.CreateClient(this._secretKey);
        //    var response = await client.GetAsync($"transaction/verify/{reference}");

        //    var json = await response.Content.ReadAsStringAsync();


        //    return JsonConvert.DeserializeObject<TransactionResponseModel>(json);
        //}


    }





//string responseBody = await response.Content.ReadAsStringAsync();
//_ = product.Quantity - model.quantity;
//Order order = new()
//{
//    ProductId = product.Id,
//    Quantity = model.quantity,
//    Price = model.Amount,
//    CustomerId = customer.Id,
//};
//Payment payment = new()
//{
//    ReferenceNumber = $"FCMS/{Guid.NewGuid().ToString()[..10]}",
//    Amount = model.Amount,
//    TransactionId = Guid.NewGuid().ToString()[..9],
//    IsPaid = true,
//    Status = Model.Enum.PaymentStatus.success,
//    CustomerId = customer.Id,
//    ProductId = product.Id,
//};

//ProductOrder porder = new()
//{
//    ProductId = product.Id,
//    OrderId = order.Id,
//};

//_orderRepository.Insert<Order>(order);
//_paymentRepository.Insert<Payment>(payment);
//_productOrderRepository.Insert<ProductOrder>(porder);
//_productRepository.Update<Product>(product);
//await _unitOfWork.SaveChangesAsync();

//return responseBody;