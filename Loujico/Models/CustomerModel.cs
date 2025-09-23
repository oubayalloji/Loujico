namespace Loujico.Models
{
    public class CustomerModel
    {
        public object Customer { get; set; }
        public List<TbFile> Files { get; set; } = new();
       
    }
}
