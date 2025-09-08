namespace Loujico.Models
{
    public class ApiResponse<T>
    {
        public string Role { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
