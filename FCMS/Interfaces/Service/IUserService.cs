using FCMS.Model.DTOs;
using FCMS.Model.Entities;

namespace FCMS.Interfaces.Service
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> GetUsers();
        Task<BaseResponse<UserDto>> Get(string Id);
        Task<BaseResponse<UserDto>> GetByMail(string Email);
        Task<BaseResponse<UserDto>> UpdateUser(UpdateUserRequestModel model);
        Task<BaseResponse<UserDto>> LoginAsync(UserLoginRequestModel model);
        Task<BaseResponse<UserDto>> Logout();
        Task<bool> DeleteAsync(string id);
    }
}
