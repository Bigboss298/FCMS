using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Repository
{
    public interface IFaqRepository : IBaseRepository
    {
        Task<Faq> Get(Expression<Func<Faq, bool>> expression);
        Task<List<Faq>> GetAllFaq();
        Task<Faq> GetFaqById(string id);
    }
}
