namespace FCMS.Model.Entities
{
    public class Chat : BaseEntity
    {
        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public bool Seen { get; set; } = false;
        public DateTime Timestamp { get; set; }
    }
}
