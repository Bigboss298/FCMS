using FCMS.Interfaces.Repository;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class ProductImageRepository : BaseRepository, IProductImageRepository
    {
        public ProductImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ProductImages>> Get(Expression<Func<ProductImages, bool>> expression)
        {
           return await _context.ProductImages.Where(expression).ToListAsync();
        }

        public async Task<IReadOnlyList<ProductImages>> GetAll(Expression<Func<ProductImages, bool>> expression)
        {
            return await _context.ProductImages.Where(expression).ToListAsync();    
        }
    }
}
