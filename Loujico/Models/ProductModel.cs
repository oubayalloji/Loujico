namespace Loujico.Models
{
    public class ProductModel
    {
        public TbProduct Product { get; set; }
        public List<TbFile> Files { get; set; } = new();
    }
}
