using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product> Get(Expression<Func<Product, bool>> expression)
        {
            return await _context.Products
                .Include(i => i.ProductImages)
                .Include(r => r.Farmer.Reviews)
                .Include(f => f.Farmer)
                .ThenInclude(u => u.User)
                .ThenInclude(a => a.Address)
                .FirstOrDefaultAsync(expression);
        }

        public async Task<IReadOnlyList<Product>> GetAll()
        {
            return (IReadOnlyList<Product>)await _context.Products
                .Include(f => f.Farmer)
                .ThenInclude(u => u.User)
                .ThenInclude(a => a.Address)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByAny(Expression<Func<Product, bool>> expression)
        {
            return (IEnumerable<Product>)await _context.Products
               .Include(f => f.Farmer)
               .ThenInclude(u => u.User)
               .ThenInclude(a => a.Address)
               .Where(expression)
               .ToListAsync();
        }
    }
}
