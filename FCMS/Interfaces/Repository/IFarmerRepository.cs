using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IFarmerRepository : IBaseRepository
    {
        Task<IReadOnlyList<Farmer>> GetAll();
        Task<Farmer> Get(Expression<Func<Farmer, bool>> expression);
    }
}
