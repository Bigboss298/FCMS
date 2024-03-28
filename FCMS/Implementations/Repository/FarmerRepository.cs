using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class FarmerRepository : BaseRepository, IFarmerRepository
    {
        public FarmerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
