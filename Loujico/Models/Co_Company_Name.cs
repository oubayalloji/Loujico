using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_Company_Name
    {
        public int Id { get; set; }
        public int? LegalId { get; set; }
        public Co_Legal? Legal { get; set; }

        [Required(ErrorMessage = "اسم الشركة مطلوب")]
        [StringLength(200)]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم السجل التجاري مطلوب")]
        [StringLength(50)]
        public string Comm_No { get; set; }

        [StringLength(50)]
        public string Tax_No { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Found_Date { get; set; }


        [Required(ErrorMessage = "وصف الشركة مطلوب")]
        [StringLength(1000, ErrorMessage = "الوصف يجب ألا يتجاوز 1000 خانة")]

        public string CompanyDescription { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastVisit { get; set; }

        public bool IsDeleted { get; set; }= false;


        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        // Navigation
        public ICollection<Co_Address> Addresses { get; set; }
        public ICollection<CompanyActivity> CompanyActivities { get; set; }

        public ICollection<Co_Contact> Contacts { get; set; }
/*        public ICollection<CompanyLegal> CompanyLegals { get; set; }
*/
        public ICollection<Co_CompanyEmployee> CompanyEmployees { get; set; }
    }


}
