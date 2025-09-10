namespace Loujico.Models
{
    public class VmUserRoles
    {
        public string userid {  get; set; }
        public string username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> roles { get; set; }
    }
}
