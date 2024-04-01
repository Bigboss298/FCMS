namespace FCMS.Model.Entities
{
    public class Farmer : BaseEntity
    {
        public string UserEmail { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string PaymentDetailId { get; set; } = default!;
        public User User { get; set; } = default!;
        public PaymentDetails PaymentDetails { get; set; }
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
