using System.Globalization;

namespace FCMS.Model.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<string> ImageUrls { get; set; } = new HashSet<string>();
        public string FarmerId { get; set; } = default!; 
        public Farmer Farmer { get; set; } = default!;
    }
}
