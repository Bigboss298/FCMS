using FCMS.FileManager;
using FCMS.Gateway.EmailService;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Enum;
using FCMS.Model.Exceptions;
using Mapster;
using Microsoft.Extensions.Hosting;
//using static Google.Protobuf.Collections.MapField<TKey, TValue>;

namespace FCMS.Implementations.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;
        private readonly IMailService _mailServices;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFileManager fileManager, IWebHostEnvironment hostEnvironment, IAddressRepository addressRepository,IMailService mailService)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
            _hostEnvironment = hostEnvironment;
            _addressRepository = addressRepository;
            _mailServices = mailService;
        }

        public async Task<BaseResponse<CustomerDto>> CreateAsync(CreateCustomerRequestModel customer)
        {
            var checkCustomer = await _userRepository.Get<User>(x => x.Email == customer.Email);
            if (checkCustomer != null)
            {
                throw new Exception($"User with the Email {customer.Email} already exists");
            }
            var newUser = customer.Adapt<User>();
            var newAddress = customer.Adapt<Address>();
                var newCustomer = new Customer
                {
                    UserEmail = customer.Email,
                    UserId = newUser.Id,
                    User = newUser,
                };

                var userImage = await _fileManager.UploadFileToSystem(customer.ProfilePicture);

                if (!userImage.Status)
                {
                    throw new Exception($"{userImage.Message}");
                };

                newUser.ProfilePicture = userImage.Data.Name;
                newUser.Customer = newCustomer;
                newUser.Address = newAddress;
                newAddress.UserId = newUser.Id;

                var mailRequest = new MailRequestDto
                {
                    Subject = "Welcome",
                    ToEmail = customer.Email,
                    ToName = customer.FirstName,
                    HtmlContent = $"<html><body><h1>Hello {customer.FirstName + "    " + customer.LastName }, Welcome to FCMS International Limited.</h1><h4> <a href=https://www.canva.com/design/DAFz_ES31XE/EtoIii982LTYgm6K-VYZgg/edit?utm_content=DAFz_ES31XE&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton></h4></body></html>"

                };

                _userRepository.Insert<User>(newUser);
                _customerRepository.Insert<Customer>(newCustomer);
                _addressRepository.Insert<Address>(newAddress);
                 _mailServices.SendEmailAsync(mailRequest);
                  await _unitOfWork.SaveChangesAsync();

                return new BaseResponse<CustomerDto>
                {
                    Message = "registration successful!",
                    Status = true,
                };
    }

        public async Task<bool> DeleteAsync(string customerId)
        {
            var customerToDelete = await _customerRepository.Get<Customer>(x => x.Id == customerId);
            var userToDelete = await _userRepository.Get<User>(x => x.Customer.Id == customerToDelete.Id);
            if (customerToDelete is null || userToDelete is null)
            {
                throw new Exception("User not found!!!");
            }

            _customerRepository.Delete<Customer>(customerToDelete);
            _userRepository.Delete<User>(userToDelete);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<BaseResponse<CustomerDto>> GetAsync(string customerId)
        {
            var customer = await _customerRepository.Get(x => x.Id == customerId);
            if(customer is null)
            {
                throw new NotFoundException($"Customer with the Id {customerId} not found");
            }

            return new BaseResponse<CustomerDto>
            {
                Message = "Customer found!!!",
                Status = true,
                Data = new CustomerDto
                {
                    CustomerId = customer.Id,
                    UserEmail = customer.UserEmail,
                    UserId = customer.User.Id,
                    User = new User
                    {
                        Id = customer.User.Id,
                        FirstName = customer.User.FirstName,
                        LastName = customer.User.LastName,
                        Email = customer.User.Email,
                        Password = customer.User.Password,
                        PhoneNumber = customer.User.PhoneNumber,
                        ProfilePicture = customer.User.ProfilePicture,
                        Gender = customer.User.Gender,
                        Role = customer.User.Role,
                        Address = new Address()
                        {
                            Country = customer.User.Address.Country,
                            City = customer.User.Address.City,
                            State = customer.User.Address.State,
                            Language = customer.User.Address.Language,
                            UserId = customer.User.Id
                        }
                    },
                },
            };
        }

        public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAll();
            if(!customers.Any())
            {
                return new List<CustomerDto>();
            }

            return customers.Select(x => new CustomerDto
            {
                UserEmail = x.UserEmail,
                UserId = x.User.Id,
                CustomerId = x.Id,
                User = new User
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    ProfilePicture = x.User.ProfilePicture,
                    Gender = x.User.Gender,
                    Role = x.User.Role,
                    Address = new Address()
                    {
                        Country = x.User.Address.Country,
                        City = x.User.Address.City,
                        State = x.User.Address.State,
                        Language = x.User.Address.Language,
                        UserId = x.User.Id
                    }
                },
            }).ToList();
        }
    }
}
