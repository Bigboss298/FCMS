namespace FCMS.Model.Entities
{
    public class Address : BaseEntity
    {
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Language { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
