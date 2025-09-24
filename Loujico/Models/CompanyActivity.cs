namespace Loujico.Models
{
    public class CompanyActivity
    {
    
            public int Id { get; set; }

            public int CompanyId { get; set; }
            public Co_Company_Name Company { get; set; }

            public int ActivityId { get; set; }
            public Co_Activity Activity { get; set; }
       
    }
}
