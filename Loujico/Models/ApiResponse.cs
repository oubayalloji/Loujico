namespace Loujico.Models
{
    public class ApiResponse<T>
    {
        public bool IsAdmin { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
