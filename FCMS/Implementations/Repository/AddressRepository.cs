using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class AddressRepository : BaseRepository, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
