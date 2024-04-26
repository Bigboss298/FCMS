namespace FCMS.Model.DTOs
{
    public class FaqDto
    {
        public string? Question { get; set; } 
        public string? Answer { get; set; }
    }

    public class CreateFaqRequestModel
    {
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
}
