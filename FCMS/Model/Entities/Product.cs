using System.Globalization;

namespace FCMS.Model.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<ProductImages>? ProductImages { get; set; }
        public string FarmerId { get; set; } = default!; 
        public Farmer Farmer { get; set; } = default!;
        public ICollection<ProductOrder> Orders { get; set; } = new HashSet<ProductOrder>();
    }

    public class ProductImages : BaseEntity
    {
        public string? ImageReference { get; set; }
        public string? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
