namespace FCMS.Model.DTOs
{
    public class ChatDto
    {
        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public bool Seen { get; set; } = false;
        public DateTime Timestamp { get; set; }
        public string LoggedInUserId { get; set; } = default!;
    }

    public class CreateChatRequestModel
    {
        public string Content { get; set; } = default!;
        public bool Seen { get; set; } = false;
        public DateTime Timestamp { get; set; }
    }
}
