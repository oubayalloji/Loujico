namespace Loujico.Models
{
    public class CompanyLegal
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public Co_Company_Name Company { get; set; }

        public int LegalId { get; set; }
        public Co_Legal Legal { get; set; }
    }
}
