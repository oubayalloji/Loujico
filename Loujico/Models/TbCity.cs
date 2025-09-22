using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class TbCity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المحافظة مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int StateId { get; set; }

        [ForeignKey("StateId")]
        public TbState State { get; set; }

        // Navigation: المحافظة فيها مدن
      
    }

}
