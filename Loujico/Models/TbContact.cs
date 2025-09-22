using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class TbContact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم نوع الاتصال مطلوب")]
        [StringLength(50)]
        public string Name { get; set; }

        // Navigation
        public ICollection<Co_Contact> CompanyContacts { get; set; }
    }
}
