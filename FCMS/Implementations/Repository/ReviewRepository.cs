using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class ReviewRepository : BaseRepository, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Review> Get(Expression<Func<Review, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Review>> GetAllFarmerReviews(Expression<Func<Review, bool>> expression)
        {
            return await _context.Reviews.Where(expression).ToListAsync();
        }
    }
}
