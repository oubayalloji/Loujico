using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_Industry
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الصناعة مطلوب")]
        [StringLength(150)]
        public string Name { get; set; }

        // Navigation
        public ICollection<Co_Activity> Activities { get; set; }
        public ICollection<CompanyActivity> CompanyActivity { get; set; }


    }

}
