using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_Address
    {
      
            public int Id { get; set; }

            [Required]
            public int CompanyId { get; set; }

            [ForeignKey("CompanyId")]
            public Co_Company_Name Company { get; set; }

            [Required]
            public int CountryId { get; set; }

            [ForeignKey("CountryId")]
            public TbCountry Country { get; set; }

            [Required]
            public int StateId { get; set; }

            [ForeignKey("StateId")]
            public TbState State { get; set; }

            [Required]
            public int CityId { get; set; }

            [ForeignKey("CityId")]
            public TbCity City { get; set; }

            [StringLength(250)]
            public string AddressLine { get; set; }
        }

    
}
