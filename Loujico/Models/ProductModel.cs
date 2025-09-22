namespace Loujico.Models
{
    public class ProductModel
    {
        public object Product { get; set; }
        public List<TbFile> Files { get; set; } = new();
    }
}
