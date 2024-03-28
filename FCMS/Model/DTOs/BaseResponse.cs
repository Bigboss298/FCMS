namespace FCMS.Model.DTOs
{
    public class BaseResponse<T>
    {
        public string Message { get; set; } = default!;
        public bool Status { get; set; }
        public T? Data { get; set; } 
    }
}
