using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository
    {
        Task<BaseResponse<User>> GetByAddress(Expression<Func<User, bool>> expression);
        Task<User> LoginAsync(UserLoginRequestModel model);
    }
}
