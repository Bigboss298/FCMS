namespace FCMS.Model.DTOs
{
    public class AddressDto
    {
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Language { get; set; } = default!;
    }

    public class UpdateAddressRequestModel
    {
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Language { get; set; } = default!;
    }
}
