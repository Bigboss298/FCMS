namespace FCMS.Model.Entities
{
    public class Faq : BaseEntity
    {
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
    }
}
