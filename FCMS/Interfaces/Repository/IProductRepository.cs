using FCMS.Implementations.Repository;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IProductRepository : IBaseRepository
    {
        Task<IReadOnlyList<Product>> GetAll();
        Task<Product> Get(Expression<Func<Product, bool>> expression);
        Task<IEnumerable<Product>> GetByAny(Expression<Func<Product, bool>> expression);
    }
}
