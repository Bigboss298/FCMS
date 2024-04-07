using FCMS.Interfaces.Repository;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class PaymentDetail : BaseRepository, IPaymentDetails
    {
        public PaymentDetail(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
