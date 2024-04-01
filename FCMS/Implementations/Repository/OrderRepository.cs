using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
