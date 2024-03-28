namespace FCMS.Model.Entities
{
    public class Admin : BaseEntity
    {
        public string UserId { get; set; } = default!; 
        public string UserName { get; set;} = default!;
        public User? User { get; set; }
    }
}
