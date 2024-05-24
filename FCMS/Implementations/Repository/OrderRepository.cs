using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Order> Get(Expression<Func<Order, bool>> expression)
        {
            return await _context.Orders
                .Include(c => c.Customer)
                .Include(p => p.Product)
                .ThenInclude(i => i.ProductImages)
                .FirstOrDefaultAsync(expression);
        }

        public async Task<IReadOnlyList<Order>> GetAll()
        {
            return await _context.Orders
              .Include(c => c.Customer)
                .Include(p => p.Product)
                .ThenInclude(i => i.ProductImages)
               .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllMyOrder(Expression<Func<Order, bool>> expression)
        {
            return await _context.Orders
               .Include(c => c.Customer)
                 .Include(p => p.Product)
                 .ThenInclude(i => i.ProductImages)
                 .Where(expression)
                .ToListAsync();
        }
    }
}
