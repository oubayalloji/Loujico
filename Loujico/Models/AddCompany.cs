using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    /* public class CompanyEditDto : AddCompany
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
     }*/
    // Create
    public class CompanyCreateDto
    {
        public string Name { get; set; } = null!;
        public string Comm_No { get; set; } = null!;
        public string? Tax_No { get; set; }
        public DateTime? Found_Date { get; set; }
        public string CompanyDescription { get; set; } = null!;
        public int? LegalId { get; set; }   // حقل جديد لربط الـ Legal إن وُجد
    }


    // Update
    public class CompanyUpdateDto
    {
        public int id {  get; set; }
        public string Name { get; set; }
        public string Comm_No { get; set; }
        public string? Tax_No { get; set; }
        public DateTime? Found_Date { get; set; }
        public string CompanyDescription { get; set; }
        public int? LegalId { get; set; }
    }

    public class CompanyReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Comm_No { get; set; } = null!;
        public string? Tax_No { get; set; }
        public DateTime? Found_Date { get; set; }
        public string CompanyDescription { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public int? LegalId { get; set; }            // إظهار الـ LegalId المرتبط
        public string? LegalInfo { get; set; }       // معلومات الـ Legal للاستجابة
    }



    public class CompanyContactCreateDto
    {
        public int ContactTypeId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CompanyContactUpdateDto
    {
        public int Id { get; set; }
        public int ContactTypeId { get; set; }
        public string Name { get; set; } = null!;
        public int CompanyId { get; set; }
    }

    public class CompanyContactReadDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactTypeName { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class CompanyActivityLinksDto
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
    public class CompanyActivityLinkDto
    {
        public int ActivityId { get; set; }
    }

    public class CompanyActivityReadDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public int IndustryId { get; set; }
        public string IndustryName { get; set; } = string.Empty;
    }
    public class CompanyEmployeeCreateDto
    {
        public int CompanyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string? Department { get; set; }
        public string? Notes { get; set; }
        public string ContactName { get; set; } // اسم وسيلة الاتصال المرتبطة بالموظف
        public int ContactTypeId { get; set; }
    }

    public class CompanyEmployeeUpdateDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string? Department { get; set; }
        public string? Notes { get; set; }
        public string ContactName { get; set; } // اسم وسيلة الاتصال المرتبطة بالموظف
        public int ContactTypeId { get; set; }
    }

    public class CompanyEmployeeReadDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string? Department { get; set; }
        public string? Notes { get; set; }
        public string ContactName { get; set; } // اسم وسيلة الاتصال المرتبطة بالموظف
        public int ContactTypeId { get; set; }  // نوع وسيلة الاتصال (هاتف، إيميل، إلخ)
    }
    public class ContactModel
    {
        public int Id { get; set; }
        public string ContactName { get; set; }
        public string ContactType { get; set; }
    }
    public class ActivityModel
    {
        public int Id { get; set; }
        public string ActivityName { get; set; }
        public string IdustryName { get; set; }
    }
}
