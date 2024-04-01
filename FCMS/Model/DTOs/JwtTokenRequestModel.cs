namespace FCMS.Model.DTOs
{
    public class JwtTokenRequestModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Role { get; set; }
        public string? UserName { get; set; }
    }
}
