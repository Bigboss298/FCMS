using FCMS.Model.Entities;
using FCMS.Model.Enum;

namespace FCMS.Model.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string ProfilePicture { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public Farmer Farmer { get; set; } = default!;
        public Address? Address { get; set; }
        public ICollection<Chat> Chats { get; set; } = new HashSet<Chat>();
        public string? JwtToken { get; set; }
    }

    public class CreateUserRequestModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public IFormFile DisplayPicture { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;
    }

    public class UpdateUserRequestModel
    {
        public string id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public IFormFile DisplayPicture { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
    }

    public class UserLoginRequestModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class UpdateUserBankDetails
    {
        public string Id { get; set; } = default!;
        public string BankCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string AccountType { get; set; } = default!;
    }

    public class UpdateUserAddress
    {
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Language { get; set; } = default!;
    }
}
