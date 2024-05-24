namespace FCMS.Gateway
{
    public interface IPaystackService
    {
        Task<string> Payment(RequestPaymentModel model);
        Task<string> PayFarmer(CreatePaymentRequestModel model);
        Task<string> GetTransactionRecieptAsync(string transactionReference);
    }
}
