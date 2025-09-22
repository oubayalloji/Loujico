using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class TbState
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المدينة مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int CountrId { get; set; }

        [ForeignKey("CountrId")]
        public TbCountry Country { get; set; }
        public ICollection<TbCity> Cities { get; set; }
    }

}
