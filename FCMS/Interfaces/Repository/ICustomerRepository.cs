using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface ICustomerRepository : IBaseRepository
    {
        Task<IReadOnlyList<Customer>> GetAll();
        Task<Customer> Get(Expression<Func<Customer, bool>> expression);
    }
}
