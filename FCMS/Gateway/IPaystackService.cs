namespace FCMS.Gateway
{
    public interface IPaystackService
    {
        Task<string> InitiatePayment(CreatePaymentRequestModel model);
        Task<string> GetTransactionRecieptAsync(string transactionReference);
    }
}
