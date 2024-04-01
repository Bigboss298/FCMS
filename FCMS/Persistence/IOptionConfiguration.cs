namespace FCMS.Persistence
{
    public class IOptionConfiguration
    {
    }
    public class PaystackOptions
    {
        public string APIKey { get; set; } = default!;
    }

    public class SendinblueOptions
    {
        public string APIKey { get; set; } = default!;
        public string SenderName { get; set; } = default!;
        public string SenderEmail { get; set; } = default!;
    }
    public class CompanyInfoOption
    {
        public string CompanyEmail { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public string AdminID { get; set; } = default!;
        public string AdminUserID { get; set; } = default!;
    }
}
