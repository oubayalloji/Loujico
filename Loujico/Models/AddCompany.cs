using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class CompanyEditDto : AddCompany
    {
        public int Id { get; set; } // مهم للتعديل
    }

    public class Co_AddressDto
    {
        public int? Id { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string? AddressLine { get; set; }
    }

    public class Co_ContactDto
    {
        public int ContactTypeId { get; set; }
        public string Name { get; set; }
    }

    public class Co_LegalDto
    {
        public int? LegalId { get; set; }     

    }

    public class Co_ActivityDto
    {
        public int ActivityId { get; set; } // مطلوب عند الإنشاء الجديد
    }

    public class Co_CompanyEmployeeDto
    {
        public int? Id { get; set; }
        [StringLength(150)] public string FirstName { get; set; }
        [StringLength(150)] public string LastName { get; set; }
        [StringLength(100)] public string Position { get; set; }

        [StringLength(500)] public string? Notes { get; set; }
        [StringLength(100)] public string Department { get; set; }
    }

    public class AddCompany
    {
        public string Name { get; set; }
        public string Comm_No { get; set; }
        public string? Tax_No { get; set; }
        public DateTime? Found_Date { get; set; }
        public string CompanyDescription { get; set; }

        public List<Co_AddressDto>? Addresses { get; set; }
        public List<Co_ContactDto>? Contacts { get; set; }
        public List<Co_LegalDto>? Legals { get; set; }
        public List<Co_ActivityDto>? Activities { get; set; }
        public List<Co_CompanyEmployeeDto>? CompanyEmployees { get; set; }
    }

}
