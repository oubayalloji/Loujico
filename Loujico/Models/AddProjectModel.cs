namespace Loujico.Models
{
    public class AddProjectModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int CustomerId { get; set; }
        public decimal? Price { get; set; }
        public int Progress { get; set; }

        public List<EmployeeOnProjectModel>? Employees { get; set; } = new List<EmployeeOnProjectModel>();
    }
}
