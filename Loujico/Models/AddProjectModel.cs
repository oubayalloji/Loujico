using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class AddProjectModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "الاسم مطلوب")]

        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "تاريخ بداية المشروع مطلوب")]

        public DateOnly StartDate { get; set; }
   

        public DateOnly? EndDate { get; set; }
        [Required(ErrorMessage = "معرف الزبون مطلوب")]

        public int CustomerId { get; set; }
        public decimal? Price { get; set; }
        public int Progress { get; set; }

        public List<EmployeeOnProjectModel>? Employees { get; set; } = new List<EmployeeOnProjectModel>();
    }
}
