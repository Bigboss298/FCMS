namespace FCMS.Model.Entities
{
    public class ProductOrder : BaseEntity
    {
        public string ProductId { get; set; } = default!;
        public string OrderId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public Order Order { get; set; } = default!;
    }
}
