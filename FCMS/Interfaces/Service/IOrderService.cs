using FCMS.Model.DTOs;
using sib_api_v3_sdk.Model;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Service
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAll();
        Task<IEnumerable<OrderDto>> GetAllMyOrderC(string param);
        Task<IEnumerable<OrderDto>> GetAllMyOrderF(string param);

        Task<BaseResponse<OrderDto>> Get(string id);
        Task<BaseResponse<OrderDto>> UpdateAsync(int param, string id);
    }
}
