namespace Loujico.Models
{
    public class ApiResponse<T>
    {
        public String Role { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
