using FCMS.Model.Entities;
using FCMS.Model.Enum;

namespace FCMS.Model.DTOs
{
    public class FarmerDto
    {
        public string UserEmail { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public ICollection<ProductDto> Products { get; set; } = new HashSet<ProductDto>();
        public User User { get; set; } = default!;
    }

    public class CreateFarmerRequestModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public IFormFile ProfilePicture { get; set; }
        public Gender Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public Farmer? Farmer { get; set; }
    }

    public class UpdateFarmerRequestModel
    {
        public string UserEmail { get; set; } = default!;
    }

    public class UpdatefarmerProduct
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<string> ImageUrls { get; set; } = new HashSet<string>();
    }
}
