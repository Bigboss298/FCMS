namespace FCMS.Gateway.EmailService
{
    public interface IMailService
    {
         void SendEmailAsync(MailRequestDto mailRequest);
    }
}
