using FCMS.Model.Entities;

namespace FCMS.Model.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<string> ImageUrls { get; set; } = new HashSet<string>();
        public string FarmerId { get; set; } = default!;
        public Farmer Farmer { get; set; } = default!;
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }

    public class UpdateProductRequestModel
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<string> Images { get; set; } = new HashSet<string>();
    }

    public class CreateProductRequestModel
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<IFormFile> Images { get; set; } = default!;
        public string FarmerId { get; set; } = default!;
    }
}
