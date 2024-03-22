namespace FCMS.Model.Entities
{
    public class Farmer : BaseEntity
    {
        public string UserEmail { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
