

using FCMS.Auth;
using FCMS.FileManager;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using Mapster;
using Paystack.Net.SDK.Models;
using ZstdSharp;

namespace FCMS.Implementations.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTManager _tokenService;
        private readonly IConfiguration _config;
        private readonly IFileManager _fileManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string generateToken = null;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IJWTManager tokenService, IConfiguration config, IFileManager fileManager, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _config = config;
            _fileManager = fileManager;
            _hostingEnvironment = hostEnvironment;
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var userToDelete = await _userRepository.Get<User>(x => x.Id == id);
            if(userToDelete is null)
            {
                throw new Exception("User not Found");
            }
            _userRepository.Delete<User>(userToDelete);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<BaseResponse<UserDto>> Get(string id)
        {
            var user = await _userRepository.Get<User>(x => x.Id == id) ?? throw new Exception("User not found!!!");
            return new BaseResponse<UserDto>
            {
                Message = "User Found",
                Status = true,
                Data = user.Adapt<UserDto>(),
            };

        }

        public async Task<BaseResponse<UserDto>> GetByMail(string email)
        {
            var user = await _userRepository.Get<User>(x => x.Email == email) ?? throw new Exception("User not found!!!");
            return new BaseResponse<UserDto>
            {
                Message = "User Found",
                Status = true,
                Data = user.Adapt<UserDto>(),
            };
        }

        public async Task<IReadOnlyList<UserDto>> GetUsers()
        {
            var users = await _userRepository.GetAll<User>();
            if(!users.Any())
            {
                return new List<UserDto>();
            }
            return users.Adapt<IReadOnlyList<UserDto>>();
        }

        public async Task<BaseResponse<UserDto>> LoginAsync(UserLoginRequestModel model)
        {
            var user = await _userRepository.LoginAsync(model);
            if(user is null)
            {
                throw new Exception("Inavlid Email or Password");
            }
            var loggedInUser = await _userRepository.Get<User>(x => x.Email == model.Email);
            var jwtModel1 = loggedInUser.Adapt<UserDto>();
            var jwtModel = jwtModel1.Adapt<JwtTokenRequestModel>();
            jwtModel.Id = loggedInUser.Id;
            jwtModel.ProfilePicture = jwtModel1.ProfilePicture;
            var token = generateToken = _tokenService.CreateToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), jwtModel);
            jwtModel1.JwtToken = token;

            user.Token = jwtModel1.JwtToken;
            _unitOfWork.SaveChangesAsync();

            return new BaseResponse<UserDto>()
            {
                Status = true,
                Message = "Login Successful!!!",
                Data = jwtModel1,
            };
        }

        public async Task<BaseResponse<UserDto>> UpdateDp(UpDateDPRequestModel model)
        {
            var getUser = await _userRepository.Get(x => x.Id == model.Id);

            if (getUser is null) throw new Exception("User not found");

            if (!string.IsNullOrEmpty(getUser.ProfilePicture))
            {
                var previousPicturePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", getUser.ProfilePicture);
                if (File.Exists(previousPicturePath))
                {
                    File.Delete(previousPicturePath);
                }
            }
            var userImage = await _fileManager.UploadFileToSystem(model.NewDp);

            if (!userImage.Status)
            {
                throw new Exception($"{userImage.Message}");
            };

            getUser.ProfilePicture = userImage.Data.Name;

            _userRepository.Update<User>(getUser);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<UserDto>
            {
                Message = "Profile picture updated sucessfully",
                Status = true,
           }; 
        }

        public async Task<BaseResponse<UserDto>> UpdatePassword(UpdatePasswordRequestModel model)
        {
            var getUser = await _userRepository.Get(x => x.Id == model.Id);
            if (getUser is null) throw new Exception("User not Found!!!");

            if (getUser.Password != model.OldPassword) throw new Exception("Old password Not corret");

            if (model.NewPassword != model.ConfirmNewPassword) throw new Exception("New password and Confirm new password doesn't match");

            getUser.Password = model.NewPassword;
            getUser.Id = model.Id;

            _userRepository.Update<User>(getUser);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<UserDto>
            {
                Message = "Password Updated Sucessfully",
                Status = true,
            };
        }

        public async Task<BaseResponse<UserDto>> UpdateUser(UpdateUserRequestModel model)
        {
           
                var userToUpdate = await _userRepository.Get<User>(x => x.Id == model.id);
                if (userToUpdate is null)   throw new Exception("User not Found");

                userToUpdate.Email = model.Email;
                userToUpdate.PhoneNumber = model.PhoneNumber;

                _userRepository.Update<User>(userToUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse<UserDto>
                {
                    Message = $"User {userToUpdate.FirstName} updated successfully",
                    Status = true,
                };
            }

       

    }
}
