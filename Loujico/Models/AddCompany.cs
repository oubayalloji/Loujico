namespace Loujico.Models
{
    public class AddCompany
    {
        // خصائص الشركة الأساسية

        public int Id { get; set; }
        public string Name { get; set; }
        public string Comm_No { get; set; }
        public string? Tax_No { get; set; }
        public DateTime? Found_Date { get; set; }
        public string CompanyDescription { get; set; }

        // القوائم كـ JSON في Form (يمكن إرسالها كحقل نصي JSON)
        public List<Co_AddressDto>? Addresses { get; set; }
        public List<Co_ContactDto>? Contacts { get; set; }
        public List<Co_LegalDto>? Legals { get; set; }
        public List<Co_ActivityDto>? Activities { get; set; }
        public List<Co_CompanyEmployeeDto>? CompanyEmployees { get; set; }
    }

}
