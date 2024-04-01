using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}