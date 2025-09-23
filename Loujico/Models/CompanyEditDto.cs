using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class CompanyEditDto : AddCompany
    {
        public int Id { get; set; } // مهم للتعديل
    
    }

    public class Co_AddressDto { public int? Id { get; set; } public int CountryId { get; set; } public int StateId { get; set; } public int CityId { get; set; } public string? AddressLine { get; set; } }
    public class Co_ContactDto { public int? Id { get; set; } public int ContactTypeId { get; set; } public string Name { get; set; } }
    public class Co_LegalDto { public int? Id { get; set; } public string LegalInfo { get; set; } }
    public class Co_ActivityDto { public int? Id { get; set; } public string Name { get; set; } public int IndustryId { get; set; } }
    public class Co_CompanyEmployeeDto 
    { public int? Id { get; set; }
        [StringLength(150)]
        public string FirstName { get; set; }

        [StringLength(150)]
        public string LastName { get; set; }
        [StringLength(100)]
        public string Position { get; set; }
        [StringLength(20)]
        [RegularExpression(@"^[0-9+\-\s]{6,20}$", ErrorMessage = "رقم الهاتف غير صالح")]
        public string? Phone { get; set; }
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [StringLength(150)]
        public string? Email { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        [StringLength(100)]
        public string Department { get; set; }
    }
}
