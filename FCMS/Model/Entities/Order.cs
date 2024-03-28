namespace FCMS.Model.Entities
{
    public class Order : BaseEntity
    {
        public string ProductId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<ProductOrder> ProductOrder { get; set; } = new HashSet<ProductOrder>();
        

    }
}
