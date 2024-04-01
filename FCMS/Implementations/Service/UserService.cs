

using FCMS.Auth;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using Mapster;

namespace FCMS.Implementations.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTManager _tokenService;
        private readonly IConfiguration _config;
        private string generateToken = null;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IJWTManager tokenService, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _config = config;
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

        public Task<BaseResponse<UserDto>> Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<UserDto>> UpdateUser(UpdateUserRequestModel model)
        {
            var userToUpdate = await _userRepository.Get<User>(x => x.Id == model.id);
            if(userToUpdate is null) throw new Exception("User not Found");
            _userRepository.Update<User>(userToUpdate.Adapt<User>());
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<UserDto>
            {
                Message = $"User {userToUpdate.FirstName} updated succesfully",
                Status = true,
                Data = userToUpdate.Adapt<UserDto>(),
            };
        }
    }
}
