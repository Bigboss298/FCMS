using FCMS.Model.Enum;

namespace FCMS.Model.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? ProfilePicture { get; set; }
        public Gender Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public Farmer? Farmer { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<PaymentDetails> PaymentDetails { get; set; } = new HashSet<PaymentDetails>();
    }
}
