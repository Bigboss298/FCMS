using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer> Get(Expression<Func<Customer, bool>> expression)
        {
            return await _context.Customers
                .Include(c => c.User)
                .ThenInclude(u => u.Address)
                .FirstOrDefaultAsync(expression);
        }

        public async Task<IReadOnlyList<Customer>> GetAll()
        {
            return (IReadOnlyList<Customer>)await _context.Customers
                .Include(x => x.User)
                .ThenInclude(a => a.Address)
                .ToListAsync();
        }
    }
}
