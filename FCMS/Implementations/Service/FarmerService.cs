using FCMS.FileManager;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;

namespace FCMS.Implementations.Service
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;


        public FarmerService(IFarmerRepository farmerRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFileManager fileManager)
        {
            _farmerRepository = farmerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
        }

        public async Task<BaseResponse<FarmerDto>> CreateAsync(CreateFarmerRequestModel model)
        {
            var farmer = await _userRepository.Get<User>(x => x.Email == model.Email);
            if(farmer != null)
            {
                throw new Exception($"User with the Email {model.Email} already exists");
            }

            var newFarmer = model.Adapt<Farmer>();
            var newUser = model.Adapt<User>();
            var farmerImage = await _fileManager.UploadFileToSystem(model.ProfilePicture);
            if(!farmerImage.Status)
            {
                throw new Exception($"{farmerImage.Message}");
            };

            newUser.ProfilePicture = farmerImage.Data.Name;
            newFarmer.User = newUser;
            newFarmer.UserId = newUser.Id;
            newFarmer.UserEmail = newUser.Email;

            _farmerRepository.Insert<Farmer>(newFarmer);
            _userRepository.Insert<User>(newUser);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<FarmerDto>
            {
                Message = "Farmer registered Successfully",
                Status = true,
                Data = newFarmer.Adapt<FarmerDto>(),
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
            var farmer = await _farmerRepository.Get<Farmer>(x => x.Id == id);
            if(farmer  is null)
            {
                throw new NotFoundException($"Farmer with the Id {id} not found");
            }
            return new BaseResponse<FarmerDto>
            {
                Message = $"Farmer with the Id {id} has been found successfully",
                Status = true,
                Data = new FarmerDto
                {
                    UserEmail = farmer.UserEmail,
                    UserId = farmer.UserId,
                    User = new User()
                    {
                        FirstName = farmer.User.FirstName,
                        LastName = farmer.User.LastName,
                        Email = farmer.User.Email,
                        PhoneNumber = farmer.User.PhoneNumber,
                        ProfilePicture = farmer.User.ProfilePicture,
                        Gender = farmer.User.Gender,
                        Role = farmer.User.Role,
                    },
                }
            };
        }

        public async Task<IReadOnlyList<FarmerDto>> GetFarmersAsync()
        {
            var farmers = await _farmerRepository.GetAll<Farmer>();
            if(!farmers.Any())
            {
                return new List<FarmerDto>();
            }
            return (IReadOnlyList<FarmerDto>)farmers.Select(x => new FarmerDto
            {
                UserEmail = x.UserEmail,
                UserId = x.UserId,
                User = new User()
                {
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    ProfilePicture = x.User.ProfilePicture,
                    Gender = x.User.Gender,
                    Role = x.User.Role,
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
