using System.ComponentModel.DataAnnotations;
using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Requests.GraphQl
{
    public class PermissionRequest
    {
        [Required]
        public string SysId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public Status Status { get; set; }

        public bool? IsNeedAprove { get; set; }

        public virtual string? ServiceKey { get; set; }

        public string? Comment { get; set; }
    
    }
}
