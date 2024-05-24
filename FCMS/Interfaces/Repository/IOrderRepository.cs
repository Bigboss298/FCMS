using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IOrderRepository : IBaseRepository
    {
        Task<IReadOnlyList<Order>> GetAll();
        Task<IEnumerable<Order>> GetAllMyOrder(Expression<Func<Order, bool>> expression);

        Task<Order> Get(Expression<Func<Order, bool>> expression);
    }
}
