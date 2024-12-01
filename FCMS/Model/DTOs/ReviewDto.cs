namespace FCMS.Model.DTOs
{
    public class ReviewDto
    {
        public int Ratings { get; set; }
        public string? Comments { get; set; }
        public string? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? FarmerId { get; set; }
    }

    public class CreateReviewRequestModel
    {
        public string FarmerId { get; set; }
        public string CustomerId { get; set; }
        public int Ratings { get; set; }
        public string? Comments { get; set;}
    }

    public class ReviewLists
    {
        public bool Status { get; set; }
        public string Message { get; set; } = default!;
        public List<ReviewDto> Data { get; set; } = new List<ReviewDto>();
    }

    public class UpdateReviewRequestModel
    {
        public int Ratings { get; set; }
        public string? Comments { get; set; }
    }






}
