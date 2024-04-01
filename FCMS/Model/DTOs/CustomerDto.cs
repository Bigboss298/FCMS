using FCMS.Model.Entities;
using FCMS.Model.Enum;

namespace FCMS.Model.DTOs
{
    public class CustomerDto
    {
        public string CustomerId { get; set; } = default!;
        public string UserEmail { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;
        public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }

    public class CreateCustomerRequestModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public IFormFile ProfilePicture { get; set; }
        public Gender Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Language { get; set; } = default!;
    }

    public class UpdateCustomerRequestModel
    {
        public string UserId { get; set; } = default!;
        public string UserEmail { get; set; } = default!;
    }

    public class UpdateCustomerOrder
    {
        public string ProductId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CustomerResponseModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? Dp { get; set; }
        public Gender Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Language { get; set; } = default!;
    }



}
