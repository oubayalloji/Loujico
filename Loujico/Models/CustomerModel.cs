namespace Loujico.Models
{
    public class CustomerModel
    {
        public TbEmployee Employee { get; set; }
        public List<TbFile> Files { get; set; } = new();
        public List<TbProjectsEmployee> Projects { get; set; } = new();
        public List<TbProductsEmployee> Products { get; set; } = new();
    }
}
