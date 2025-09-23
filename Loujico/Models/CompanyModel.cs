namespace Loujico.Models
{
    public class CompanyModel
    {
        public Co_Company_Name Company { get; set; }
        public List<TbFile> Files { get; set; } = new();

    }
}
