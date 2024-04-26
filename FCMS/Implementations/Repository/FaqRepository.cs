using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class FaqRepository : BaseRepository, IFaqRepository
    {
        public FaqRepository(ApplicationDbContext context) : base(context)
        {
        }


        public Task<Faq> Get(Expression<Func<Faq, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Faq>> GetAllFaq()
        {
            return await _context.Faqs.ToListAsync();
        }

        public async Task<Faq> GetFaqById(string id)
        {
            return await _context.Faqs.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
