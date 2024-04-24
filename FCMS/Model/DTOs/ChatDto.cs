namespace FCMS.Model.DTOs
{
    public class ChatDto
    {
        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public bool Seen { get; set; } = false;
        public DateTime Timestamp { get; set; }
        public string? LoggedInUserId { get; set; }
    }

    public class ChatLists
    {
        public bool Status { get; set; }
        public string Message { get; set; } = default!;
        public List<ChatDto> Data { get; set; } = new List<ChatDto>();
    }

    public class CreateChatRequestModel
    {
        public string Content { get; set; } = default!;
    }
}
