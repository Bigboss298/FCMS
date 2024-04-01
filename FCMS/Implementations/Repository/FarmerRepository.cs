using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class FarmerRepository : BaseRepository, IFarmerRepository
    {
        public FarmerRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<Farmer> Get(Expression<Func<Farmer, bool>> expression)
        {
            return await _context.Farmers
                .Include(c => c.User)
                .ThenInclude(u => u.Address)
                .FirstOrDefaultAsync(expression);
        }

        public async Task<IReadOnlyList<Farmer>> GetAll()
        {
            return (IReadOnlyList<Farmer>)await _context.Farmers
                .Include(x => x.User)
                .ThenInclude(a => a.Address)
                .ToListAsync();
        }
    }
}
