using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class PaymentDetails : BaseRepository, IPaymentDetails
    {
        public PaymentDetails(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
