using FCMS.Gateway;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Enum;
using FCMS.Model.Exceptions;
using System.Linq.Expressions;

namespace FCMS.Implementations.Service
{
    public class OrderServices : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaystackService _paystackService;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;


        public OrderServices(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IPaystackService paystackService, IUserRepository userRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _paystackService = paystackService;
            _userRepository = userRepository;
            _customerRepository = customerRepository;

        }

        public async Task<BaseResponse<OrderDto>> Get(string id)
        {
            var orderToGet = await _orderRepository.Get(x => x.Id == id);
            if(orderToGet is null)
            {
                throw new NotFoundException("Order Not Found!!!");
            }

            return new BaseResponse<OrderDto>
            {
                Status = true,
                Message = "Order Found",
                Data = new OrderDto
                {
                    Id = orderToGet.Id,
                    ProductId = orderToGet.ProductId,
                    PaymentId = orderToGet.PaymentId,
                    Product = new Product
                    {
                        Name = orderToGet.Product.Name,
                        Description = orderToGet.Product.Description,
                        ProductImages = orderToGet?.Product?.ProductImages?.Select(x => new ProductImages
                        {
                            ImageReference = x.ImageReference,
                        }).ToList(),
                        FarmerId = orderToGet.Product.FarmerId,
                    },
                    Status = orderToGet.Status,
                    Quantity = orderToGet.Quantity,
                    Price = orderToGet.Price,
                    CustomerId = orderToGet.CustomerId,
                    DateCreated = orderToGet.DateCreated,
                },
            };
        }

        public async Task<IEnumerable<OrderDto>> GetAll()
        {
            var orders = await _orderRepository.GetAll();
            if (!orders.Any()) return new List<OrderDto>();
            return orders.Select(x => new OrderDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                PaymentId = x.PaymentId,
                Product = new Product
                {
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    Quantity = x.Product.Quantity,
                    Price = x.Product.Price,
                    ProductImages = x?.Product?.ProductImages?.Select(x => new ProductImages
                    {
                        ImageReference = x.ImageReference,
                    }).ToList(),
                    FarmerId = x.Product.FarmerId,
                },
                Status = x.Status,
                Quantity = x.Quantity,
                Price = x.Price,
                CustomerId = x.CustomerId,
                DateCreated = x.DateCreated,
            }).ToList();
        }

        public async Task<IEnumerable<OrderDto>> GetAllMyOrder(string param)
        {
            var cust = await _customerRepository.Get(x => x.UserId == param);
            if (cust is null) throw new NotFoundException("User not found");
            var orders = await _orderRepository.GetAllMyOrder(x => x.CustomerId == cust.Id);
            if (!orders.Any()) return new List<OrderDto>();
            return orders.Select(x => new OrderDto
            {
                Id = x.Id,
                Product = new Product
                {
                    Name = x.Product.Name,
                    ProductImages = x?.Product?.ProductImages?.Select(x => new ProductImages
                    {
                        ImageReference = x.ImageReference,
                    }).ToList(),
                },
                Status = x.Status,
                Quantity = x.Quantity,
                Price = x.Price,
                DateCreated = x.DateCreated,
            }).ToList();
        }

        public async Task<BaseResponse<OrderDto>> UpdateAsync(int param, string id)
        {
            var orderToUpdate = await _orderRepository.Get(x => x.Id == id);
            if (orderToUpdate is null)
            {
                throw new NotFoundException("Order Not Found!!!");
            }

            orderToUpdate.Status = (OrderStatus)param;

            _orderRepository.Update<Order>(orderToUpdate);
            await _unitOfWork.SaveChangesAsync();

            var newPayFarmerModel = new CreatePaymentRequestModel
            {
                CustomerId = orderToUpdate.CustomerId,
                productId = orderToUpdate.ProductId,
                OrderId = orderToUpdate.Id,
            };

            if (orderToUpdate.Status == OrderStatus.Confirmed)
            {
                await _paystackService.PayFarmer(newPayFarmerModel);
            }

            return new BaseResponse<OrderDto>
            {
                Status = true,
                Message = $"Farmer has been paid Successfully",
            };

        }
    }
}
