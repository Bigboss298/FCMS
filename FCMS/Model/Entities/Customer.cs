namespace FCMS.Model.Entities
{
    public class Customer : BaseEntity
    {
        public string UserEmail { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;
        public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
