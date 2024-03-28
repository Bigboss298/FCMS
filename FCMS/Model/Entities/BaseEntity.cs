
namespace FCMS.Model.Entities
{
    public class BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString()[..7];
        public string DateCreated { get; set; } = DateTime.Now.Date.ToShortDateString();
        public object User { get; internal set; }
    }
}
