using FCMS.Model.DTOs;

namespace FCMS.Interfaces.Service
{
    public interface IFaqService
    {
        Task<BaseResponse<FaqDto>> GetFaqById(string id);
        Task<List<FaqDto>> GetAllFaqs();
        Task<BaseResponse<FaqDto>> CreateAsync(CreateFaqRequestModel model);
    }
}
