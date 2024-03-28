using FCMS.FileManager;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using Mapster;
using Microsoft.Extensions.Hosting;

namespace FCMS.Implementations.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFileManager fileManager, IWebHostEnvironment hostEnvironment)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<BaseResponse<CustomerDto>> CreateAsync(CreateCustomerRequestModel customer)
        {
            var checkCustomer = await _userRepository.Get<User>(x => x.Email == customer.Email);
            if (checkCustomer != null)
            {
                throw new Exception($"User with the Email {customer.Email} already exists");
            }

            if (customer.ProfilePicture is null || customer.ProfilePicture.Length <= 0)
            {
                throw new Exception("Upload your profile picture");
                //return new BaseResponse<CustomerDto>()
                //{
                //    Status = false,
                //    Message = "Upload your profile picture",
                //};
            }
            var acceptableExtension = new List<string>() { ".jpg", ".jpeg", ".png", ".dnb" };
            var fileExtension = Path.GetExtension(customer.ProfilePicture.FileName);
            if (!acceptableExtension.Contains(fileExtension))
            {
                throw new Exception("File format not suppported, please upload any of the following format (jpg, jpeg, png, dnb)");
                //return new BaseResponse<CustomerDto>()
                //{
                //    Status = false,
                //    Message = "File format not suppported, please upload any of the following format (jpg, jpeg, png, dnb)"
                //};
            }
            var newUser = customer.Adapt<User>();
                var newCustomer = new Customer
                {
                    UserEmail = customer.Email,
                    UserId = newUser.Id,
                    User = newUser,
                };

                var userImage = ManageUploadOfProfilePictures(customer.ProfilePicture);

                newUser.ProfilePicture = userImage;
                newUser.Customer = newCustomer;

                _userRepository.Insert<User>(newUser);
                _customerRepository.Insert<Customer>(newCustomer);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse<CustomerDto>
                {
                    Message = "registration successful!",
                    Status = true,
                    Data = newCustomer.Adapt<CustomerDto>(),
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
            var customer = await _customerRepository.Get<Customer>(x => x.Id == customerId);
            if(customer is null)
            {
                return new BaseResponse<CustomerDto>
                {
                    Message = "No such customer",
                    Status = false,
                };
            }

            return new BaseResponse<CustomerDto>
            {
                Message = "Customer found!!!",
                Status = true,
                Data = new CustomerDto
                {
                    UserEmail = customer.UserEmail,
                    User = new User
                    {
                        Id = customer.UserId,
                        FirstName = customer.User.FirstName,
                        LastName = customer.User.LastName,
                        Email = customer.User.Email,
                        Password = customer.User.Password,
                        PhoneNumber = customer.User.PhoneNumber,
                        ProfilePicture = customer.User.ProfilePicture,
                        Gender = customer.User.Gender,
                        Role = customer.User.Role,
                    }
                },
            };
        }

        public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAll<Customer>();
            if(!customers.Any())
            {
                return new List<CustomerDto>();
            }
            
           return customers.Adapt<IReadOnlyList<CustomerDto>>();
        }



        private string ManageUploadOfProfilePictures(IFormFile picture)
        {
            var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "ProfilePictures");
            Directory.CreateDirectory(uploadsFolderPath);
            string fileName = Guid.NewGuid() + picture.FileName;
            string photoPath = Path.Combine(uploadsFolderPath, fileName);

            using (var fileStream = new FileStream(photoPath, FileMode.Create))
            {
                picture.CopyTo(fileStream);
            }

            return fileName;
        }


    }
}
