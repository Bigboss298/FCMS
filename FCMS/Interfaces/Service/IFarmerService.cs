using FCMS.Model.DTOs;

namespace FCMS.Interfaces.Service
{
    public  interface IFarmerService
    {
        Task<BaseResponse<FarmerDto>> GetAsync(string id);
        Task<BaseResponse<FarmerDto>> UpdateAsync(string id, UpdateFarmerRequestModel model);
        Task<IReadOnlyList<FarmerDto>> GetFarmersAsync();
        Task<bool> DeleteAsync(string id);
        Task<BaseResponse<FarmerDto>> CreateAsync(CreateFarmerRequestModel model);
    }
}
