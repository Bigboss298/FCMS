using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class ProductOrderRepository : BaseRepository, IProductOrderRepository
    {
        public ProductOrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
