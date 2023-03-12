using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Requests.Short
{
    public class RoleShort
    {

        [Required]
        public string SysId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public bool? IsNeedAprove { get; set; }

        public string? Comment { get; set; }

        public virtual List<string>? Permissions { get; set; }
    }
}
