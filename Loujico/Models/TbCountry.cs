using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class TbCountry
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الدولة مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation
        public ICollection<TbState> States { get; set; }
    }
}
