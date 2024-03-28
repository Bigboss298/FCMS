using FCMS.Model.DTOs;

namespace FCMS.Interfaces.Service
{
    public interface ICustomerService
    {
        Task<BaseResponse<CustomerDto>> GetAsync(string customerId);
        Task<IReadOnlyList<CustomerDto>> GetCustomersAsync();
        //Task<BaseResponse<CustomerDto>> UpdateAsync(CustomerDto customer);
        Task<BaseResponse<CustomerDto>> CreateAsync(CreateCustomerRequestModel customer);
        Task<bool> DeleteAsync(string customerId);
    }
}
