using FCMS.FileManager;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FCMS.Implementations.Service
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;
        private readonly IAddressRepository _addressRepository;
        private static HttpClient _client;
        private readonly string _secretKey;
        private readonly IPaymentDetails _paymentDetails;


        public FarmerService(IConfiguration configuration, IFarmerRepository farmerRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFileManager fileManager, IAddressRepository addressRepository, IPaymentDetails paymentDetails)
        {
            _farmerRepository = farmerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
            _addressRepository = addressRepository;
            _secretKey = configuration["Paystack:ApiKey"];
            _client = new HttpClient();
            _paymentDetails = paymentDetails;
        }

        public async Task<BaseResponse<FarmerDto>> CreateAsync(CreateFarmerRequestModel model)
        {

            if (model is null) throw new BadRequestException($"Model can't be null");
            var checkFarmer = await _userRepository.Get<User>(x => x.Email == model.Email);
            if(checkFarmer != null)
            {
                throw new Exception($"User with the Email {model.Email} already exists");
            }

            var newUser = model.Adapt<User>();
            var newAddress = model.Adapt<Address>();
            var newFarmer = new Farmer
            {
                UserEmail = newUser.Email,
                UserId = newUser.Id,
                User = newUser,
            };

            var farmerImage = await _fileManager.UploadFileToSystem(model.ProfilePicture);
            if(!farmerImage.Status)
            {
                throw new Exception($"{farmerImage.Message}");
            };

            newUser.ProfilePicture = farmerImage.Data.Name;
            newUser.Farmer = newFarmer;
            newFarmer.User = newUser;
            newFarmer.UserId = newUser.Id;
            newUser.Address = newAddress;
            newAddress.UserId = newUser.Id;


            _farmerRepository.Insert<Farmer>(newFarmer);
            _userRepository.Insert<User>(newUser);
            _addressRepository.Insert<Address>(newAddress);

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<FarmerDto>
            {
                Message = "Farmer registered Successfully",
                Status = true
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var farmerToDelete = await _farmerRepository.Get<Farmer>(x => x.Id == id);
            var userToDelete = await _userRepository.Get<User>(x => x.Farmer.Id == id);
            if(farmerToDelete is null || userToDelete is null)
            {
                throw new Exception("User doesnt exits!!!");
            }
            _farmerRepository.Delete<Farmer>(farmerToDelete);
            _userRepository.Delete<User>(userToDelete);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<BaseResponse<FarmerDto>> GetAsync(string id)
        {
            var farmer = await _farmerRepository.Get(x => x.Id == id);
            if(farmer  is null)
            {
                throw new NotFoundException("Farmer not Found!!!");
            }
            return new BaseResponse<FarmerDto>
            {
                Message = $"Farmer with the Id {id} has been found successfully",
                Status = true,
                Data = new FarmerDto
                {
                    FarmerId = farmer.Id,
                    UserEmail = farmer.UserEmail,
                    UserId = farmer.User.Id,
                    User = new User
                    {
                        Id = farmer.Id,
                        FirstName = farmer.User.FirstName,
                        LastName = farmer.User.LastName,
                        Email = farmer.User.Email,
                        Password = farmer.User.Password,
                        PhoneNumber = farmer.User.PhoneNumber,
                        ProfilePicture = farmer.User.ProfilePicture,
                        Gender = farmer.User.Gender,
                        Role = farmer.User.Role,
                        Address = new Address()
                        {
                            Country = farmer.User.Address.Country,
                            City = farmer.User.Address.City,
                            State = farmer.User.Address.State,
                            Language = farmer.User.Address.Language,
                            UserId = farmer.User.Id
                        }
                    },
                },
            };
        }

        public async Task<IReadOnlyList<FarmerDto>> GetFarmersAsync()
        {
            var farmers = await _farmerRepository.GetAll();
            if(!farmers.Any())
            {
                return new List<FarmerDto>();
            }
            return farmers.Select(x => new FarmerDto
            {
                FarmerId = x.Id,
                UserEmail = x.UserEmail,
                UserId = x.UserId,
                User = new User()
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

        public async Task<BaseResponse<FarmerDto>> UpdateAsync(string id, UpdateFarmerRequestModel model)
        {
            var farmer = await _farmerRepository.Get<Farmer>(x => x.Id == id);
            if(farmer is null)
            {
                return new BaseResponse<FarmerDto>
                {
                    Message = $"No farmer with the Id {id} exist",
                    Status = false,
                };
            }
            farmer.UserEmail = model.UserEmail;
            _farmerRepository.Update<Farmer>(farmer);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<FarmerDto>
            {
                Message = $"Farmer {farmer.User.FirstName} profile Updated Succesfully",
                Status = true,
            };

        }



    }
}
