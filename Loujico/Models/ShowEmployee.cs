using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class ShowEmployeeModel
    {
        public TbEmployee Employee { get; set; }
        public List<TbFile> Files { get; set; } = new();

    }

}
