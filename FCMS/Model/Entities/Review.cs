using System.Runtime.CompilerServices;

namespace FCMS.Model.Entities
{
    public class Review : BaseEntity
    {
        public int? Ratings { get; set; }
        public string? Comments { get; set; }
        public string? CustomerId { get; set; }
        public string? FarmerId { get; set; }

    }
}
