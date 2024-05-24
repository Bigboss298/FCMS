using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IProductImageRepository : IBaseRepository
    {
        Task<List<ProductImages>> Get(Expression<Func<ProductImages, bool>> expression);
        Task<IReadOnlyList<ProductImages>> GetAll(Expression<Func<ProductImages, bool>> expression);
    }
}
