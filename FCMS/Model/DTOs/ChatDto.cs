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
        public List<ChatDto> Data { get; set; } = new List<ChatDto>();
    }

    public class CreateChatRequestModel
    {
        public string Content { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string RecieverId { get; set; } = default!;
    }
    public class MyChatsDto
    {
        public string UserId { get; set; } = default!;
        public string UserFirstName { get; set; } = default!;
        public string UserLastName { get; set; } = default!;
        public int UnSeenChats { get; set; }
    }
}
