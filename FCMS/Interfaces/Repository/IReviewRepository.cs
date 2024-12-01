using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IReviewRepository : IBaseRepository
    {
        Task<Review> Get(Expression<Func<Review, bool>> expression);
        Task<IEnumerable<Review>> GetAllFarmerReviews(Expression<Func<Review, bool>> expression);
    }
}
