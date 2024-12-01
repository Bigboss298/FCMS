using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Service
{
    public interface IReviewService
    {
        Task<BaseResponse<ReviewDto>> Get(string id);
        Task<IEnumerable<ReviewDto>> GetAllFarmerReviews(string farmerId);
        Task<BaseResponse<ReviewDto>> DeleteAllReviews(string farmerId);
        Task<BaseResponse<ReviewDto>> Add(CreateReviewRequestModel model);
        Task<BaseResponse<ReviewDto>> Update(string id, UpdateReviewRequestModel model);
        Task<BaseResponse<ReviewDto>> Delete(string id);
    }
}
