using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_Legal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "المعلومة القانونية مطلوبة")]
        [StringLength(200)]
        public string LegalInfo { get; set; }

        public ICollection<Co_CompanyEmployee> Company { get; set; }

    }
}
